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
    public static class EncryptionHelper
    {
        public static CipherMode CipherModeFromString(string stringMode)
        {
            CipherMode mode = CipherMode.CBC;
            switch (stringMode)
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

            return mode;
        }

        public static byte[] GenerateSessionKey(int keySize = 128)
        {
            byte[] sessionKey;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();
                sessionKey = (aes.Key).ToArray();
            }
            Console.WriteLine("dlugosc klucza " + sessionKey.Length + " , klucz " + sessionKey.ToString());
            return sessionKey;
        }

        //based on: https://stackoverflow.com/questions/1307204/how-to-generate-unique-public-and-private-key-via-rsa
        public static void GenerateKeyPairRSA(out string publicKey, out string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    publicKey = rsa.ToXmlString(false);
                    privateKey = rsa.ToXmlString(true);

                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        //returns encrypted session key with given public key (public key is a string in xml form)
        //encrypted key is returned as string
        public static string EncryptSessionKeyToString(byte[] sessionKey, string publicKey)
        {
            byte[] encryptedKeyBytes = RSAEncrypt(sessionKey, publicKey);
            string encryptedKeyString = Convert.ToBase64String(encryptedKeyBytes);

            return encryptedKeyString;
        }

        //given encrypted session key (a string from header of encrypted file) 
        //and a private key (string in xml form)
        //returns session key in byte array
        public static byte[] DecryptSessionKeyFromString(string encryptedKey, string privateKey)
        {
            byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKey);
            byte[] decryptedKeyBytes = RSADecrypt(encryptedKeyBytes, privateKey);
            return decryptedKeyBytes;
        }

        //based on: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(v=vs.110).aspx
        public static byte[] RSAEncrypt(byte[] dataToEncrypt, string publicKey)
        {
            byte[] encryptedData = null;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(publicKey);
                    encryptedData = rsa.Encrypt(dataToEncrypt, false);

                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return encryptedData;
        }

        //based on: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(v=vs.110).aspx
        public static byte[] RSADecrypt(byte[] dataToDecrypt, string privateKey)
        {
            byte[] decryptedData = null;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(privateKey);
                    decryptedData = rsa.Decrypt(dataToDecrypt, false);

                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return decryptedData;
        }
    }
}
