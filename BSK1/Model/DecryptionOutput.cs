using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class DecryptionOutput
    {
        public DecryptionOutput(bool isInputCorrect, 
            string inputFilePath, string outputFilePath, 
            User recipent, string errorMsg)
        {
            IsInputCorrect = isInputCorrect;
            EncryptedFile = inputFilePath;
            DecryptedFile = outputFilePath;
            Recipent = recipent;
            ErrorMessage = errorMsg;
        }

        public bool IsInputCorrect { get; set; }
        public string EncryptedFile { get; set; }
        public string DecryptedFile { get; set; }
        public User Recipent { get; set; }
        public string ObtainedPassword { get; set; }
        public string ErrorMessage { get; set; }

    }
}
