using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    static class Globals
    {

        public static String AppMainDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CryptoVirus";
        public static String PathToPrivateKeysDir = AppMainDirPath + "\\NotReallyPrivateKeys";
        public static String PswrdsFilePath = AppMainDirPath + "\\pswrds";
        public static String PublicKeysFilePath = AppMainDirPath + "\\public";
        public static String PrivateKeysFilePath = PathToPrivateKeysDir + "\\private";


        public static String XmlMainElement = "EncryptedFileHeader";
        public static String XmlAlgorithm = "Algorithm";
        public static String XmlKeySize = "KeySize";
        public static String XmlBlockSize = "BlockSize";
        public static String XmlCipherMode = "CipherMode";
        public static String XmlIV = "IV";
        public static String XmlExtension = "FileExtension";
        public static String XmlApprovedUsers = "ApprovedUsers";
        public static String XmlUser = "User";
        public static String XmlEmail = "Email";
        public static String XmlSessionKey = "SessionKey";
        public static String XmlPublicKey = "PublicKey";
        public static String XmlPrivateKey = "PrivateKey";
        public static String XmlPassword = "Password";


        public static String UsersXmlFilePath = "users.xml";
        public static String UsersNode = "Users";

        public static String Algorithm = "AES";
        public static String statusMsgEncryption = "Status: szyfrowanie";
        public static String statusMsgDecryption = "Status: deszyfracja";

        public static String statusMsgEncryptionFinished = "Status: plik zaszyfrowany";
        public static String statusMsgDecryptionFinished = "Status: plik odszyfrowany";

        public static String encryptionFinishedPopup = "Szyfrowanie zakończone sukcesem";
        public static String decryptionFinishedPopup = "Deszyfracja zakończona sukcesem";


        public static int blockSize = 128;
        public static int keySize = 128;

        public const String modeECB = "ECB";
        public const String modeCBC = "CBC";
        public const String modeCFB = "CFB";
        public const String modeOFB = "OFB";

    }
}
