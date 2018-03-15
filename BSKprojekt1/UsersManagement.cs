using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BSKprojekt1
{
    public static class UsersManagement
    {
        //reads xml file with structure 
        //<User> 
        //  <Email> </Email>
        //  <PublicKey> </PublicKey>
        //</User>
        //and returns users list
        //todo create that file and read from it! it's dummy data for now
        public static List<User> GetUsersListFromFile(string usersAndKeysXmlFilePath)
        {
            List<User> users = new List<User>();

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
    }
}
