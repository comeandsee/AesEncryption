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
        public static void DecryptFile(string filePath, string decodedFileName, byte[] key)
        {
            Console.WriteLine("decrypting");
            XmlHelpers.RetrieveXmlHeaderFromFile(filePath, out string xmlHeaderString, out long headerByteLength);

            XmlHelpers.ReadDataFromXMLHeader(xmlHeaderString,
                out string algorithm, out string keySize,
                out string blockSize, out string cipherMode,
                out string iv, out Dictionary<string, string> recipents,
                out string fileExtension);

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
            /*
            EncryptionHelper.AesDecryptToFile(filePath, 
                decodedFileName + fileExtension, key, mode, Int32.Parse(keySize), Convert.FromBase64String(iv));
                */
        }
    }
}
