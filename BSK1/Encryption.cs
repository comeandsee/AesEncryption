using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class Encryption
    {
        public Encryption(string fileToBeEncrypted, string encryptedFile, 
            int blockSize, string cipherModeString, string fileExtension, 
            List<User> recipents)
        {
            FileToBeEncrypted = fileToBeEncrypted;
            EncryptedFile = encryptedFile;
            BlockSize = blockSize;
            FileExtension = fileExtension;
            Recipents = recipents;
            CipherMode = 
                EncryptionHelper.CipherModeFromString(cipherModeString);

            IV = null;
        }

        public string FileToBeEncrypted { get; set; }
        public string EncryptedFile { get; set; }
        public int BlockSize { get; set; }
        public CipherMode CipherMode { get; set; }
        public string FileExtension { get; set; }
        public List<User> Recipents { get; set; }


        private byte[] IV { get; set; }

        public void GenerateEncodedFile(BackgroundWorker worker)
        {
            //generate session key
            byte[] sessionKey = EncryptionHelper.GenerateSessionKey(Globals.keySize);

            //generate a dictionary with pairs
            //<user email, session key encrypted with that user public key>
            Dictionary<string, string> emailKeyDictionary = 
                EncryptSessionKeyForAllRecipents(sessionKey);

            string tempEncodedFile = "tempEncoded";
            //encrypt file with sessionKey
            //show progress of encryption with worker (bgworker)
            //save encrypted file as tempEncodedFile file
            EncryptFile(sessionKey, tempEncodedFile, worker);

            string tempHeaderFile = "tempHeader";
            //take all data necessary to decrypt encrypted file
            //and save it as xml file tempHeaderFile
            CreateFileHeader(tempHeaderFile, emailKeyDictionary);

            //saves header and encrypted file as EncryptedFile
            MergeHeaderAndEncryptedFile(tempHeaderFile, tempEncodedFile);
        }

        private void MergeHeaderAndEncryptedFile(string headerFile, 
            string encryptedFile)
        {
            //create out file
            using (Stream destStream = File.Create(EncryptedFile))
            {
                //save header to file
                using (Stream srcStream = File.OpenRead(headerFile))
                {
                    srcStream.CopyTo(destStream);
                }

                //insert two blank lines between header and contents
                byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
                destStream.Write(newLine, 0, 1);
                destStream.Write(newLine, 0, 1);
                
                //append encrypted contents to out file
                using (Stream srcStream = File.OpenRead(encryptedFile))
                {
                    srcStream.CopyTo(destStream);
                }
            }
        }

        //creates header with all data needed to encrypt tempEncoded file
        //saves it in tempHeaderFile file
        private void CreateFileHeader(string tempHeaderFile, 
            Dictionary<string, string> emailKeyDictionary)
        {
            string cipherModeString = CipherMode.ToString();
            string ivString = Convert.ToBase64String(IV);

            XmlHelpers.GenerateXMLHeader(tempHeaderFile, Globals.Algorithm,
                Globals.keySize.ToString(), Globals.blockSize.ToString(), 
                cipherModeString, ivString, emailKeyDictionary, FileExtension);

        }

        //encrypts input file (fileToBeEncoded)
        //with given session key
        //saves in file of name tempEncodedFile
        //based on https://msdn.microsoft.com/pl-pl/library/system.security.cryptography.aes(v=vs.110).aspx
        private void EncryptFile(byte[] sessionKey, string tempEncodedFile,
            BackgroundWorker worker)
        {
        
            using (Aes aesAlg = Aes.Create())
            {
                //set session key
                aesAlg.Key = sessionKey;
                                
                //set data as given by the user
                aesAlg.Mode = CipherMode;
                aesAlg.BlockSize = BlockSize;

                //set initialization vector
                aesAlg.GenerateIV();
                //todo this is not pretty, is it
                IV = (aesAlg.IV).ToArray();
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                //based on: https://stackoverflow.com/questions/9237324/encrypting-decrypting-large-files-net
                using (FileStream destFileStream = 
                        new FileStream(tempEncodedFile, FileMode.Create,
                            FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = 
                        new CryptoStream(destFileStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (FileStream source = new FileStream(FileToBeEncrypted, 
                            FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            Stopwatch stopwatch = Stopwatch.StartNew();


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
                            stopwatch.Stop();
                            Console.WriteLine("encryption time " + stopwatch.ElapsedMilliseconds);

                        }
                    }
                }

            
            }
        }

        //takes recipents list (List<User> Recipents)
        //and sessionKey
        //returns a dictionary with pairs 
        //<user email, session key encrypted with that user public key>
        private Dictionary<string, string> EncryptSessionKeyForAllRecipents(byte[] sessionKey)
        {
            Dictionary<string, string> recipentsKeysDict = new Dictionary<string, string>();
            string encryptedKey;

            foreach (User recipent in Recipents)
            {
                encryptedKey = EncryptionHelper.EncryptSessionKeyToString(sessionKey, recipent.publicRSAKey);
                recipentsKeysDict.Add(recipent.Email, encryptedKey);
            }

            return recipentsKeysDict;
        }

        
    }
}
