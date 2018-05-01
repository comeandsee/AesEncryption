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
        public Decryption(string encryptedFile, 
            string decryptedFile, User selectedUser)
        {
            EncryptedFile = encryptedFile;
            DecryptedFile = decryptedFile;
            SelectedUser = selectedUser;
        }

        public string EncryptedFile { get; set; }
        public string DecryptedFile { get; set; }
        public User SelectedUser { get; set; }

        public int BlockSize { get; set; }
        public CipherMode CipherMode { get; set; }
        public string FileExtension { get; set; }
        public Dictionary<string, string> RecipentsEmailSessionKey { get; set; }
        public byte[] IV { get; set; }

        public void Decrypt(BackgroundWorker worker)
        {
            long headerLengthInBytes = ManageHeader();
            byte[] decryptedSessionKeyBytes = ManageRecipent();

            DecryptFile(headerLengthInBytes, decryptedSessionKeyBytes);
        }

        //decrypts EncryptedFile file, start reading the file at 
        //headerLengthInBytes byte
        //saves decrypted file to DecryptedFile file
        private void DecryptFile(long headerLengthInBytes, byte[] sessionKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = sessionKey;
                aesAlg.Mode = CipherMode;
                aesAlg.BlockSize = BlockSize;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                //based on: https://stackoverflow.com/questions/9237324/encrypting-decrypting-large-files-net
                using (FileStream destination = new FileStream(DecryptedFile, FileMode.CreateNew,
                    FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write))
                    {
                        try
                        {
                            using (FileStream source = new FileStream(EncryptedFile, FileMode.Open,
                                FileAccess.Read, FileShare.Read))
                            {
                                //start reading from after header
                                source.Position = headerLengthInBytes;
                                source.CopyTo(cryptoStream);
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

        private byte[] ManageRecipent()
        {
            //recipents are kept in a dictionary as 
            //<recipentEmail, encryptedUserSessionKey> pairs

            //to decrypt the file we need a session key
            //we need to find selectedUser- the user that current user of the app claims to be-
            //get their encryptedUserSessionKey
            //and decrypt the key using user's private key

            //todo maybe set to some noise, so that if foreach doesn't find anything, the decoding will work and produce noise-file
            string encryptedSessionKeyString = "err";
            foreach (KeyValuePair<string, string> emailKey in RecipentsEmailSessionKey)
            {
                if (emailKey.Key.Equals(SelectedUser.Email))
                {
                    encryptedSessionKeyString = emailKey.Value;
                    break;
                }
            }

            //decrypt session key using user's private key
            string userPrivateKeyString = UsersManagement.GetUserPrivateKeyFromFile(SelectedUser.Email);
            
            byte[] decryptedSessionKeyBytes = EncryptionHelper.DecryptSessionKeyFromString(encryptedSessionKeyString, userPrivateKeyString);
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
                XmlHelpers.RetrieveXmlHeaderFromFile(EncryptedFile);

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
            DecryptedFile += FileExtension;

            long headerLengthInBytes = DecryptionHelpers.GetHeaderLength(EncryptedFile);

            return headerLengthInBytes;           

        }

       
    }
}
