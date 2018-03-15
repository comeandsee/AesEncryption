using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BSKprojekt1
{
    //TODO- make it as linq to xml
    //at the moment it doesn't work good with users in the header
    //it nests next user in the previous one- make it right somehow
    public static class XmlHelpers
    {
        public static void GenerateXMLHeader(string outputFileName, string algorithm, 
            string keySize, string blockSize, string cipherMode, String iv, Dictionary<string, string> recipents)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            using (XmlWriter writer = XmlWriter.Create(outputFileName, settings))
            {
                writer.WriteStartElement(Globals.XmlMainElement);
                writer.WriteElementString(Globals.XmlAlgorithm, algorithm);
                writer.WriteElementString(Globals.XmlKeySize, keySize);
                writer.WriteElementString(Globals.XmlBlockSize, blockSize);
                writer.WriteElementString(Globals.XmlCipherMode, cipherMode);
                writer.WriteElementString(Globals.XmlIV, iv);

                writer.WriteStartElement(Globals.XmlApprovedUsers);

                foreach (KeyValuePair<string, string> recipent in recipents)
                {
                    writer.WriteStartElement(Globals.XmlUser);
                    writer.WriteElementString(Globals.XmlEmail, recipent.Key);
                    writer.WriteElementString(Globals.XmlSessionKey, recipent.Value);
                }

            }
        }
    }
}
