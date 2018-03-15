using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    static class Globals
    {
        public static String XmlMainElement = "EncryptedFileHeader";
        public static String XmlAlgorithm = "Algorithm";
        public static String XmlKeySize = "KeySize";
        public static String XmlBlockSize = "BlockSize";
        public static String XmlCipherMode = "CipherMode";
        public static String XmlIV = "IV";
        public static String XmlApprovedUsers = "ApprovedUsers";
        public static String XmlUser = "User";
        public static String XmlEmail = "Email";
        public static String XmlSessionKey = "SessionKey";
        public static String XmlPublicKey = "PublicKey";


        public static String UsersXmlFilePath = "users.xml";
        public static String UsersNode = "Users";

        public static String Algorithm = "AES";

        public static int blockSize = 128;
        
    }
}
