using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    public static class Encryption
    {
        public static void GenerateEncodedFile(string inputFilePath, 
            string outputFilePath, int blockSize, string cipherMode, 
            string fileExtension, List<User> recipents,
            BackgroundWorker worker)
        {
            int keySizeBits = 128; 
           
            //generate session file
            byte[] sessionKey = EncryptionHelper.GenerateSessionKey(keySizeBits);

            //get a dictionary with recipent emails and their encrypted session keys
            Dictionary<string, string> recipentsKeysDict = Encryption.GetRecipentsEncryptedSessionKeys(sessionKey, recipents);

            //initialization vector is set in AesEncryptFromFile and then put into header
            byte[] IV = null;

            string tempEncodedFile = "tempEncoded.xml";

            //encrypting input file and saving it in destined out file
            using (Aes myAes = Aes.Create())
            {
                EncryptionHelper.AesEncryptFromFile(inputFilePath, tempEncodedFile, myAes.Key, myAes.Mode, myAes.BlockSize, out IV, worker);
                //Encryption.DecryptToFile(pathToOutFile, decodedFileName, myAes.Key, myAes.Mode, myAes.BlockSize, IV);
            }

            string ivString = Convert.ToBase64String(IV);

            string tempFileWithHeader = "tempHeader.xml";
            XmlHelpers.GenerateXMLHeader(tempFileWithHeader, Globals.Algorithm,
                keySizeBits.ToString(), blockSize.ToString(), cipherMode, ivString, recipentsKeysDict, fileExtension);

            MergeHeaderAndEncodedContentIntoOutputFile(outputFilePath, tempFileWithHeader, tempEncodedFile);

        }

        //combines contents from headerFile and encodedFile to outputFile 
        private static void MergeHeaderAndEncodedContentIntoOutputFile(string outputFile, string headerFile, string encodedFile)
        {

            using (Stream destStream = File.Create(outputFile))
            {
                using (Stream srcStream = File.OpenRead(headerFile))
                {
                    srcStream.CopyTo(destStream);
                }

                //insert two blank lines between header and contents
                byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
                destStream.Write(newLine, 0, 1);
                destStream.Write(newLine, 0, 1);

                using (Stream srcStream = File.OpenRead(encodedFile))
                {
                    srcStream.CopyTo(destStream);
                }
            }
            
        }

        public static Dictionary<string,string> GetRecipentsEncryptedSessionKeys(byte[] sessionKey, List<User> recipents)
        {
            Dictionary<string, string> recipentsKeysDict = new Dictionary<string, string>();
            string encryptedKey;

            foreach(User recipent in recipents)
            {
                encryptedKey = EncryptionHelper.EncryptSessionKeyToString(sessionKey, recipent.publicRSAKey);
                recipentsKeysDict.Add(recipent.Email, encryptedKey);
            }

            return recipentsKeysDict;
        }
    }
}
