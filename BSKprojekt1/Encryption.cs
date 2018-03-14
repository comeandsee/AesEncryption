using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    public static class Encryption
    {
        public static void GenerateEncodedFile(string inputFilePath, string outputFilePath)
        {
            using (Aes myAes = Aes.Create())
            {
                byte[] IV;

                EncryptionHelper.AesEncryptFromFile(inputFilePath, outputFilePath, myAes.Key, myAes.Mode, myAes.BlockSize, out IV);
                //Encryption.DecryptToFile(pathToOutFile, decodedFileName, myAes.Key, myAes.Mode, myAes.BlockSize, IV);
            }
        }
    }
}
