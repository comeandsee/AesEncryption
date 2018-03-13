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
        //encrypts file from path srcFileName to file in path destFileName
        public static void EncryptToBytes(string srcFileName, string destFileName, 
            byte[] key, CipherMode mode, int blockSize, out byte[] IV)
        {
            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();
                aesAlg.Mode = mode;
                aesAlg.BlockSize = blockSize;

                IV = (aesAlg.IV).ToArray();
                
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                //based on: https://stackoverflow.com/questions/9237324/encrypting-decrypting-large-files-net
                using (FileStream destFileStream = new FileStream(destFileName, FileMode.CreateNew, 
                    FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(destFileStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (FileStream source = new FileStream(srcFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            source.CopyTo(cryptoStream);
                        }
                    }
                }


                /*
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        //when encoding strings
                         (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(fileText);
                        }
                        encrypted = msEncrypt.ToArray();
                        
                        
                    }
                }*/
            }
        }

        public static void DecryptStringFromBytes(string encodedFileName, string decodedFileName, byte[] key, CipherMode mode, int blockSize, byte[] IV)
        {
            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = mode;
                aesAlg.BlockSize = blockSize;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                //based on: https://stackoverflow.com/questions/9237324/encrypting-decrypting-large-files-net
                using (FileStream destination = new FileStream(decodedFileName, FileMode.CreateNew, 
                    FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write))
                    {
                        try
                        {
                            using (FileStream source = new FileStream(encodedFileName, FileMode.Open, 
                                FileAccess.Read, FileShare.Read))
                            {
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
                /*using (MemoryStream msDecrypt = new MemoryStream(cipherText))
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
                }*/


            }

        }
    }
}
