using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class EncryptionInput
    {
        public EncryptionInput(string inputFile, 
            string outputFile, string cipherMode, IList listItems)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
            CipherMode = cipherMode;
            ListItems = listItems;
        }

        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string CipherMode { get; set; }
        public System.Collections.IList ListItems { get; set; }


    }
}
