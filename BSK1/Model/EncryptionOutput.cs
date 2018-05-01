using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class EncryptionOutput
    {
        public EncryptionOutput(bool isInputCorrect,
            string inputFilePath, string outputFilePath, 
            string cipherMode, string fileExtension, List<User> recipents,
            string errorMsg)
        {
            IsInputCorrect = isInputCorrect;
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            CipherMode = cipherMode;
            FileExtension = fileExtension;
            Recipents = recipents;
            ErrorMessage = errorMsg;
        }

        public bool IsInputCorrect { get; set; }
        public string ErrorMessage { get; set; }
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string CipherMode { get; set; }
        public string FileExtension { get; set; }
        public List<User> Recipents { get; set; }
    }
}
