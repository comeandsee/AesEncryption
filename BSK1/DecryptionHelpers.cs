using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public static class DecryptionHelpers
    {
        //returns byte length of header from xml file (encryptedFile)
        public static long GetHeaderLength(string encryptedFile)
        {
            int bufferSize = 128;
            byte[] data = new byte[bufferSize];
            long bytesRead = 0;
            long headerSizeInBytes = 0;
            bool headerRead = false;

            using (BinaryReader reader = new BinaryReader(
                new FileStream(encryptedFile, FileMode.Open)))
            {
                //reads bufferSize number of bytes from encryptedFile
                //when it reaches substring "0D0D" in hex representation
                //of byte line- it has reached the end of header
                //so it calculates exact position of that byte 
                //and returns it
                //substring "0D0D" represents two new lines
                while ((reader.Read(data, 0, bufferSize) != 0) && !headerRead)
                {
                    bytesRead += bufferSize;

                    string hex = BitConverter.ToString(data).Replace("-", "");
                    //todo is 0d0d always the newline newline?
                    int indexOf0D = hex.IndexOf("0D0D");
                    if (indexOf0D != -1)
                    {
                        headerSizeInBytes = bytesRead - bufferSize +
                            (indexOf0D + 4) / 2;
                        headerRead = true;
                       // Console.Write(indexOf0D + "|");
                    }
                   // Console.WriteLine(bytesRead + ": " + hex);
                }

            }
            //Console.WriteLine("header size " + headerSizeInBytes);

           /*
            *code below reads bufferSize bytes of encrypted content
            using (BinaryReader reader = new BinaryReader(
                new FileStream(encryptedFile, FileMode.Open)))
            {
                reader.BaseStream.Seek(headerSizeInBytes, SeekOrigin.Begin);
                reader.Read(data, 0, bufferSize);

                string hex = BitConverter.ToString(data);
                Console.WriteLine(hex);
            }*/

            return headerSizeInBytes;
        }
    }
}
