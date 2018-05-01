using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BSK1
{
    //TODO- make it as linq to xml
    //at the moment it doesn't work good with users in the header
    //it nests next user in the previous one- make it right somehow
    public static class XmlHelpers
    {

        public static void GenerateXMLHeader(string outputFileName, string algorithm,
            string keySize, string blockSize, string cipherMode, string iv, Dictionary<string, string> recipents,
            String fileExtension)
        {

            //create all main nodes
            XElement algoElem = new XElement(Globals.XmlAlgorithm, algorithm);
            XElement keySizeElem = new XElement(Globals.XmlKeySize, keySize);
            XElement blockSizeElem = new XElement(Globals.XmlBlockSize, blockSize);
            XElement cipherModeElem = new XElement(Globals.XmlCipherMode, cipherMode);
            XElement ivElem = new XElement(Globals.XmlIV, iv);
            XElement extensionElem = new XElement(Globals.XmlExtension, fileExtension);


            //create approved users node
            XElement approvedUsersElem = new XElement(Globals.XmlApprovedUsers);
            XElement userElem;
            foreach (KeyValuePair<string, string> recipent in recipents)
            {
                userElem = new XElement(Globals.XmlUser,
                    new XElement(Globals.XmlEmail, recipent.Key),
                    new XElement(Globals.XmlSessionKey, recipent.Value));

                approvedUsersElem.Add(userElem);
            }

            //create main node and document itself
            XElement mainNode = new XElement(Globals.XmlMainElement);
            mainNode.Add(algoElem);
            mainNode.Add(keySizeElem);
            mainNode.Add(blockSizeElem);
            mainNode.Add(cipherModeElem);
            mainNode.Add(ivElem);
            mainNode.Add(extensionElem);
            mainNode.Add(approvedUsersElem);

            XDocument xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                mainNode);


            xDoc.Save(outputFileName);


        }


        public static string RetrieveXmlHeaderFromFile(string encryptedFile)
        {
            string xmlHeaderString = "";

            StringBuilder sb = new StringBuilder();
            int bufferSize = 128;
            bool readingHeader = true;

            using (var fileStream = File.OpenRead(encryptedFile))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
                {
                    String line;
                    while (readingHeader && (line = streamReader.ReadLine()) != null)
                    {
                        //we are before the new line separating header and encoded file
                        //so add that line (of header) to header string
                        sb.Append(line);


                        if (string.IsNullOrWhiteSpace(line))
                        {
                            //we have reached the new line, so the whole header is read
                            readingHeader = false;
                            xmlHeaderString = sb.ToString();

                           
                        }

                    }
                    
                }
            }
            return xmlHeaderString;
        }

        public static void ReadDataFromXMLHeader(string xmlHeaderString,
            out string algorithm, out string keySize, out string blockSize,
            out string cipherMode, out string iv,
            out Dictionary<string, string> recipents,
            out string fileExtension)
        {
            recipents = new Dictionary<string, string>();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xmlHeaderString);
            }
            catch (XmlException e)
            {
                Console.WriteLine("cannot read encrypted file");
                fileExtension = "err";
                blockSize = "err";
                cipherMode = "err";
                algorithm = "err";
                iv = "err";
                keySize = "err";
                return;
            }

            algorithm = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlAlgorithm).InnerText;

            keySize = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlKeySize).InnerText;

            blockSize = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlBlockSize).InnerText;

            cipherMode = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlCipherMode).InnerText;

            iv = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlIV).InnerText;

            fileExtension = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlExtension).InnerText;


            //todo maaybe check if there are any?
            string userEmail, encryptedUserSessionKey;
            XmlNode recipentsNode = doc.DocumentElement
                .SelectSingleNode("/" + Globals.XmlMainElement + "/" + Globals.XmlApprovedUsers);
            foreach (XmlNode recipentNode in recipentsNode.ChildNodes)
            {
                userEmail = recipentNode[Globals.XmlEmail].InnerText;
                encryptedUserSessionKey = recipentNode[Globals.XmlSessionKey].InnerText;
                recipents.Add(userEmail, encryptedUserSessionKey);
            }
        }
        
    }
}
