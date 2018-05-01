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
        public static void DecryptFile(EncryptionObject eo, string filePath, string decodedFileName, User selectedUser)
        {
            Console.WriteLine("decrypting");

            //todo temp
            /*string tempEncodedFilePath = "tempEncodedContents";
            XmlHelpers.RetrieveXmlHeaderFromFile(filePath, out string xmlHeaderString, tempEncodedFilePath);

            XmlHelpers.ReadDataFromXMLHeader(xmlHeaderString,
                out string algorithm, out string keySize,
                out string blockSize, out string cipherMode,
                out string iv, out Dictionary<string, string> recipents,
                out string fileExtension);

    */
            //recipents are kept in a dictionary as 
            //<recipentEmail, encryptedUserSessionKey> pairs

            //to decrypt the file we need a session key
            //we need to find selectedUser- the user that current user of the app claims to be-
            //get their encryptedUserSessionKey
            //and decrypt the key using user's private key

            //todo maybe set to some noise, so that if foreach doesn't find anything, the decoding will work and produce noise-file

            //todo temp
            /*string encryptedSessionKeyString ="aaaa";
            foreach(KeyValuePair<string, string> emailKey in recipents)
            {
                if (emailKey.Key.Equals(selectedUser.Email))
                {
                    encryptedSessionKeyString = emailKey.Value;
                    break;
                }
            }

            //decrypt session key using user's private key
            string userPrivateKeyString = UsersManagement.GetUserPrivateKeyFromFile(selectedUser.Email);
            
            byte[] decryptedSessionKeyByte = EncryptionHelper.DecryptSessionKeyFromString(encryptedSessionKeyString, userPrivateKeyString);
            */

            //todo temp to remove
            string userPrivateKeyString = UsersManagement.GetUserPrivateKeyFromFile(selectedUser.Email);

            byte[] decryptedSessionKeyByte = EncryptionHelper.DecryptSessionKeyFromString(eo.encryptedSessionKey, userPrivateKeyString);
            Console.WriteLine("private key of " + selectedUser.Email + " is " + userPrivateKeyString);

            CipherMode mode = CipherMode.CBC;

            EncryptionHelper.AesDecryptToFile(filePath,
                decodedFileName + ".txt",
                decryptedSessionKeyByte, mode, eo.blockSize,
                Convert.FromBase64String(eo.ivString));

        
            //set cipher mode
            //todo uncomment
            /*CipherMode mode = CipherMode.CBC;

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
            
            EncryptionHelper.AesDecryptToFile(tempEncodedFilePath, 
                decodedFileName + fileExtension, 
                decryptedSessionKeyByte, mode, Int32.Parse(keySize), //not key size, should be block size!
                Convert.FromBase64String(iv));
                */
        }
    }
}
