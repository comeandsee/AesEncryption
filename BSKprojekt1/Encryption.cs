using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    public static class Encryption
    {
        //based on https://msdn.microsoft.com/pl-pl/library/system.security.cryptography.aes(v=vs.110).aspx
        public static byte[] EncryptToBytes(string fileText, byte[] key, CipherMode mode, int blockSize, out byte[] IV)
        {
            byte[] encrypted;

            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();
                aesAlg.Mode = mode;
                aesAlg.BlockSize = blockSize;

                IV = (aesAlg.IV).ToArray();
                
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(fileText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
        

            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, CipherMode mode, int blockSize, byte[] IV)
        {
            string plainText = null;

            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = mode;
                aesAlg.BlockSize = blockSize;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, 
                        decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = 
                            new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            
        }

            return plainText;
        }
    }
}
