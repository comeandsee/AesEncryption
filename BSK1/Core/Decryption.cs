using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BSK1
{
    public class Decryption
    {
        DecryptionOutput DecryptionOutput { get; set; }
        public Decryption(DecryptionOutput decryptionOutput)
        {
            DecryptionOutput = decryptionOutput;
        }
        
        public int BlockSize { get; set; }
        public CipherMode CipherMode { get; set; }
        public string FileExtension { get; set; }
        public Dictionary<string, string> RecipentsEmailSessionKey { get; set; }
        public byte[] IV { get; set; }

        public void Decrypt(BackgroundWorker worker)
        {
            long headerLengthInBytes = ManageHeader();
            byte[] decryptedSessionKeyBytes = 
                ManageRecipent(DecryptionOutput.ObtainedPassword);

            try
            {
                DecryptFile(headerLengthInBytes, decryptedSessionKeyBytes, worker);
            }catch(Exception e)
            {
                //can't decrypt file, so create a file with encrypted contents
                //and change extension
                Console.WriteLine("generating wrong file");
                GenerateWrongFile(headerLengthInBytes, worker);
            }
        }

        //based on: https://stackoverflow.com/questions/3914445/how-to-write-contents-of-one-file-to-another-file
        private void GenerateWrongFile(long headerLengthInBytes, BackgroundWorker worker)
        {
            using (FileStream stream = File.OpenRead(
                DecryptionOutput.EncryptedFile))
            using (FileStream writeStream = File.OpenWrite(
                DecryptionOutput.DecryptedFile))
            {
                //start reading from after the header
                stream.Position = headerLengthInBytes;
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(writeStream);

                // create a buffer to hold the bytes 
                int bufferSize = 1024;
                byte[] buffer = new Byte[bufferSize];
                int bytesRead, count = 1;
                double progress;

                // while the read method returns bytes
                // keep writing them to the output stream
                while ((bytesRead =
                        stream.Read(buffer, 0, bufferSize)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    progress = ((double)count * bufferSize / stream.Length) * 100;
                    worker.ReportProgress((int)progress);
                    count++;
                }
            }
        }

        //decrypts EncryptedFile file, start reading the file at 
        //headerLengthInBytes byte
        //saves decrypted file to DecryptedFile file
        private void DecryptFile(long headerLengthInBytes, byte[] sessionKey, BackgroundWorker worker)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = sessionKey;
                aesAlg.Mode = CipherMode;
                aesAlg.BlockSize = BlockSize;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                //based on: https://stackoverflow.com/questions/9237324/encrypting-decrypting-large-files-net
                using (FileStream destination = new FileStream(
                    DecryptionOutput.DecryptedFile, FileMode.CreateNew,
                    FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream =
                        new CryptoStream(destination, decryptor, CryptoStreamMode.Write))
                    {
                        try
                        {
                            using (FileStream source = new FileStream(
                                DecryptionOutput.EncryptedFile, FileMode.Open,
                                FileAccess.Read, FileShare.Read))
                            {
                                //start reading from after header
                                source.Position = headerLengthInBytes;

                                int bitsInBuffer = 64 * 1024;
                                byte[] buffer = new byte[bitsInBuffer];//todo decide on size
                                int data, count = 1;
                                double progress;
                                while ((data = source.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    cryptoStream.Write(buffer, 0, data);
                                    progress = ((double)count * bitsInBuffer / source.Length) * 100;
                                    worker.ReportProgress((int)progress);
                                    count++;
                                }


                            }
                        }
                        catch (CryptographicException exception)
                        {
                            if (exception.Message == "Padding is invalid and cannot be removed.")
                                throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                            else
                                throw;
                        }
                    }
                }        
            }
        }

        private byte[] ManageRecipent(string obtainedPassword)
        {
            //recipents are kept in a dictionary as 
            //<recipentEmail, encryptedUserSessionKey> pairs

            //to decrypt the file we need a session key
            //we need to find selectedUser- the user that current user of the app claims to be-
            //get their encryptedUserSessionKey
            //and decrypt the key using user's private key

            //todo maybe set to some noise, so that if foreach doesn't find anything, the decoding will work and produce noise-file
            string encryptedSessionKeyString = "err";
            bool userIsAuthorizedToDecrypt = false;
            foreach (KeyValuePair<string, string> emailKey in RecipentsEmailSessionKey)
            {
                if (emailKey.Key.Equals(DecryptionOutput.Recipent.Email))
                {
                    encryptedSessionKeyString = emailKey.Value;
                    userIsAuthorizedToDecrypt = true;
                    break;
                }
            }

            //if selected user is not on recipents list- they have no right to decrypt file
            //so generate random session key
            if (!userIsAuthorizedToDecrypt)
            {
                Console.WriteLine("user can't decode that file! generating random session key");
                return null;
            }

            //decrypt session key using user's private key
            string userPrivateKeyString = 
                UsersManagement.GetUserPrivateKey(DecryptionOutput.Recipent.Email, obtainedPassword);
            
            byte[] decryptedSessionKeyBytes = 
                EncryptionHelper.DecryptSessionKeyFromString(encryptedSessionKeyString, userPrivateKeyString);
            return decryptedSessionKeyBytes;
        }

        //gets xml header from EncryptedFile
        //parses data from that header
        //gets length of the header in bytes
        //todo for now- sets params of this object (cipher mode, block size, iv
        //and dict and extension!- maybe
        //make it as a new object and pass around?)
        private long ManageHeader()
        {
            //get xml header from EncryptedFile
            string xmlHeaderString =
                XmlHelpers.RetrieveXmlHeaderFromFile(DecryptionOutput.EncryptedFile);

            //parse data from header
            XmlHelpers.ReadDataFromXMLHeader(xmlHeaderString,
                out string algorithm, out string keySize,
                out string blockSize, out string cipherMode,
                out string iv, out Dictionary<string, string> recipents,
                out string fileExtension);

            //todo manage that somehow
            BlockSize = Int32.Parse(blockSize);
            CipherMode = EncryptionHelper.CipherModeFromString(cipherMode);
            IV = Convert.FromBase64String(iv);
            RecipentsEmailSessionKey = recipents;
            FileExtension = fileExtension;
            DecryptionOutput.DecryptedFile += FileExtension;

            long headerLengthInBytes = 
                DecryptionHelpers.GetHeaderLength(DecryptionOutput.EncryptedFile);

            return headerLengthInBytes;           

        }

       
    }
}
