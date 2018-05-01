using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class DecryptionInput
    {
        public DecryptionInput(string inputFile, string outputFileName, User recipent)
        {
            InputFile = inputFile;
            OutputFileName = outputFileName;
            Recipent = recipent;
        }

        public string InputFile { get; set; }
        public string OutputFileName { get; set; }
        public User Recipent { get; set; }

    }
}
