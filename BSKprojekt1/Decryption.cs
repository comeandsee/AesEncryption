using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    public static class Decryption
    {
        public static void DecryptFile(string filePath, string decodedFileName, User selectedUser)
        {
            Console.WriteLine("decrypting");
            XmlHelpers.RetrieveXmlHeaderFromFile(filePath, out string xmlHeaderString, out long headerByteLength);

            XmlHelpers.ReadDataFromXMLHeader(xmlHeaderString,
                out string algorithm, out string keySize,
                out string blockSize, out string cipherMode,
                out string iv, out Dictionary<string, string> recipents,
                out string fileExtension);
            //recipents are kept in a dictionary as 
            //<recipentEmail, encryptedUserSessionKey> pairs

            //to decrypt the file we need a session key
            //we need to find selectedUser- the user that current user of the app claims to be-
            //get their encryptedUserSessionKey
            //and decrypt the key using user's private key

            //todo maybe set to some noise, so that if foreach doesn't find anything, the decoding will work and produce noise-file
            string encryptedSessionKeyString ="aaaa";
            foreach(KeyValuePair<string, string> emailKey in recipents)
            {
                if (emailKey.Key.Equals(selectedUser.Email))
                {
                    encryptedSessionKeyString = emailKey.Value;
                    break;
                }
            }

            //encrypt session key using user's private key
            string userPrivateKeyString = UsersManagement.GetUserPrivateKeyFromFile(selectedUser.Email);

            byte[] decryptedSessionKeyByte = EncryptionHelper.DecryptSessionKeyFromString(encryptedSessionKeyString, userPrivateKeyString);


            //set cipher mode
            CipherMode mode = CipherMode.CBC;
            switch (cipherMode)
            {
                case Globals.modeCBC:
                    mode = CipherMode.CBC;
                    break;
                case Globals.modeCFB:
                    mode = CipherMode.CFB;
                    break;
                case Globals.modeECB:
                    mode = CipherMode.ECB;
                    break;
                case Globals.modeOFB:
                    mode = CipherMode.OFB;
                    break;

            }
            
            EncryptionHelper.AesDecryptToFile(filePath, 
                decodedFileName + fileExtension, decryptedSessionKeyByte, mode, Int32.Parse(keySize), Convert.FromBase64String(iv));
                
        }
    }
}
