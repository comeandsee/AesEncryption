using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public static class PasswordManager
    {


        //based on: https://msdn.microsoft.com/pl-pl/library/system.security.cryptography.aes(v=vs.110).aspx
        public static byte[] EncryptBytesAesECB(string dataToEncrypt, byte[] key)
        {
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                //set IV to all 0's
                //length set according to https://msdn.microsoft.com/pl-pl/library/system.security.cryptography.symmetricalgorithm.iv(v=vs.110).aspx
                byte[] IV = new byte[aesAlg.BlockSize / 8];
                Array.Clear(IV, 0, IV.Length);
                aesAlg.IV = IV;

                aesAlg.Mode = CipherMode.ECB;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(dataToEncrypt);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }
            }

            return encryptedBytes;

        }

        //based on: https://msdn.microsoft.com/pl-pl/library/system.security.cryptography.aes(v=vs.110).aspx
        public static string DecryptBytesAesECB(byte[] encryptedData, byte[] key)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                byte[] IV = new byte[aesAlg.BlockSize / 8];
                Array.Clear(IV, 0, IV.Length);
                aesAlg.IV = IV;

                aesAlg.Mode = CipherMode.ECB;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;

        }
    }
}
