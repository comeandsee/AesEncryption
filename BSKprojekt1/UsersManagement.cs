using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BSKprojekt1
{
    public static class UsersManagement
    {
        public static List<User> users = new List<User>();
        //reads xml file with structure 
        //<User> 
        //  <Email> </Email>
        //  <PublicKey> </PublicKey>
        //</User>
        //and returns users list
        //todo create that file and read from it! it's dummy data for now
        public static List<User> GetUsersListFromFile(string usersAndKeysXmlFilePath)
        {

            string publicKey, privateKey;
            EncryptionHelper.GenerateKeyPairRSA(out publicKey, out privateKey);
            users.Add(new User("p@r.com", publicKey));

            EncryptionHelper.GenerateKeyPairRSA(out publicKey, out privateKey);
            users.Add(new User("m@r.com", publicKey));

            /*XmlNode userEmail, userPublicKey;
            List<User> users = new List<User>();
            User user;

            XmlDocument doc = new XmlDocument();
            doc.Load(Globals.UsersXmlFilePath);

            XmlNode usersNode = doc.DocumentElement.
                SelectSingleNode("//" + Globals.UsersNode);

            if (usersNode == null)
            {
                Console.WriteLine("there is no users node");
                return null;
            }

            //for each user
            foreach (XmlNode node in usersNode.ChildNodes)
            {
                userEmail = node[Globals.XmlEmail];
                userPublicKey = node[Globals.XmlPublicKey];

                user = new User(userEmail.InnerText, userPublicKey.InnerText);
                users.Add(user);
                Console.WriteLine("added " + user.Email + ", key: " + user.publicRSAKey);

            }
            */
            return users;

        }

        public static void AddUser(String email, String password)
        {
            EncryptionHelper.GenerateKeyPairRSA(out string publicKey, out string privateKey);
            //todo maaybe check if user already exists
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    Console.WriteLine("public key " + publicKey);
                    rsa.FromXmlString(publicKey);//todo start from here- powinno teoretycznie zainicjalizować obiekt rsa z tego klucza- czy działa nie wiem
                    //ponadto to nie jest ten sam klucz co zapisany jakby nie patrzeć- kodowanie może być inne i < to &lt;
                    Console.WriteLine("all good");

                }
                catch
                {
                    Console.WriteLine("sth went wrong");
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
                
            }



            users.Add(new User(email, publicKey));

            //add user to all xml files
            //pswrds(.xml): 
            //  <Users>
            //      <User><Email></Email><Password></Password></User>
            //  </Users>
            //public(.xml): 
            //  <Users>
            //      <User><Email></Email><PublicKey></PublicKey></User>
            //  </Users> 
            //private(.xml): 
            //  <Users>
            //      <User><Email></Email><PrivateKey></PrivateKey></User>
            //  </Users>  

            /*
            AddUserToCorrectFile(email, password, Globals.PswrdsFilePath, Globals.XmlPassword);
            AddUserToCorrectFile(email, publicKey, Globals.PublicKeysFilePath, Globals.XmlPublicKey);
            AddUserToCorrectFile(email, privateKey, Globals.PrivateKeysFilePath, Globals.XmlPrivateKey);
            */


        }
        
        private static void AddUserToCorrectFile(string userEmail, string value, string filePath, string valueNodeName)
        {
            XDocument doc;

            //create user node
            XElement newUserNode = new XElement(Globals.XmlUser,
                    new XElement(Globals.XmlEmail, userEmail),
                    new XElement(valueNodeName, value));


            //if file doesn't exist- create it
            if (!(File.Exists(filePath)))
            {
                XElement mainNode = new XElement(Globals.UsersNode);
                doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    mainNode);
            }
            else
            {
                doc = XDocument.Load(filePath);
            }

            //add new user to xml file
            doc.Element(Globals.UsersNode).Add(newUserNode);
            doc.Save(filePath);

        }

        //based on: https://stackoverflow.com/questions/5859632/regular-expression-for-password-validation
        public static bool PasswordCorrect(string password)
        {
            const int MIN_LENGTH = 8;

            if (string.IsNullOrEmpty(password)) return false;

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH;
            bool hasLetter = false;
            bool hasDecimalDigit = false;
            bool hasSpecialChar = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsLetter(c))
                    {
                        hasLetter = true;
                    }
                    else if (char.IsDigit(c))
                    {
                        hasDecimalDigit = true;
                    }else if (!char.IsWhiteSpace(c))
                    {
                        //if a char isn't a letter or a digit it has to be
                        //either a space or special character
                        hasSpecialChar = true;
                    }
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasLetter
                        && hasDecimalDigit
                        && hasSpecialChar
                        ;
            return isValid;

        }
    }
}
