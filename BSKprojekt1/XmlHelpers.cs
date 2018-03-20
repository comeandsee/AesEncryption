using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BSKprojekt1
{
    //TODO- make it as linq to xml
    //at the moment it doesn't work good with users in the header
    //it nests next user in the previous one- make it right somehow
    public static class XmlHelpers
    {
        
        public static void GenerateXMLHeader(string outputFileName, string algorithm,
            string keySize, string blockSize, string cipherMode, String iv, Dictionary<string, string> recipents,
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
    }
}
