using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public static class InputHandlers
    {
        public static EncryptionOutput ParseEncryptionValues(EncryptionInput ei)
        {
            bool readingAllOK = true;
            string errorMsg = "";

            //retrieve input file and output file name
            string inputFilePath = ei.InputFile;
            if (string.IsNullOrEmpty(inputFilePath)||
                !File.Exists(inputFilePath))
            {
                readingAllOK = false;
                errorMsg += "Błędnie wybrany plik do zaszyfrowania.\n";

            }

            //get input file extension
            string fileExtension = "";
            try
            {
                fileExtension = System.IO.Path.GetExtension(inputFilePath);

            }catch(ArgumentException e)
            {
                readingAllOK = false;
                errorMsg += "Błędne rozszerzenie pliku wejściowego.\n";
            }

            //get output file name
            string outputFileName = ei.OutputFile;
            if (string.IsNullOrEmpty(outputFileName) ||
                (outputFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
            {
                readingAllOK = false;
                errorMsg += "Błędna nazwa pliku wyjściowego.\n";

            }

            string outputFilePath = "";
            try
            {
                string outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
                outputFilePath = outDirectory + "\\" + outputFileName;
            }
            catch(Exception e)
            {
                readingAllOK = false;
                errorMsg += "Błędny katalog pliku wejściowego.\n";

            }

            //retrieve cipher mode
            string cipherMode = ei.CipherMode;
            

            //retrieve selected recipents from listbox
            System.Collections.IList items = ei.ListItems;
            List<User> recipents = items.Cast<User>().ToList();

            if(recipents == null || recipents.Count == 0)
            {
                readingAllOK = false;
                errorMsg += "Brak wybranych odbiorców pliku.\n";
            }

            //create and return an object keeping track of all read data
            EncryptionOutput eo = new EncryptionOutput(readingAllOK,
                inputFilePath, outputFilePath, cipherMode, fileExtension,
                recipents, errorMsg);

            return eo;
        }

        public static DecryptionOutput ParseDecryptionValues(DecryptionInput ei)
        {
            bool readingAllOK = true;
            string outDirectory = "";
            string decodedFileName = "";
            string errorMsg = "";

            //retrieve input file and output file name
            string inputFilePath = ei.InputFile;
            if (string.IsNullOrEmpty(inputFilePath)||
                !File.Exists(inputFilePath))
            {
                readingAllOK = false;
                errorMsg += "Błędnie wybrany plik do zaszyfrowania.\n";
            }
            else
            {
                outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
            }

            string outputFileName = ei.OutputFileName;
            string outputFilePath = "";
            if (string.IsNullOrEmpty(outputFileName) || 
                (outputFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
            {
                readingAllOK = false;
                errorMsg += "Błędna nazwa pliku wyjściowego.\n";

            }
            else
            {
                outputFilePath = outDirectory + "\\" + outputFileName;
                decodedFileName = outDirectory + "\\result.txt";
            }


            //retrieve selected recipent of encoded file from listbox
            User recipent = ei.Recipent;
            if (recipent == null)
            {
                readingAllOK = false;
                errorMsg += "Brak wybranego odbiorcy pliku.\n";
            }
            
            //create and return an object keeping track of all read data
            DecryptionOutput decryptionOutput = new DecryptionOutput(readingAllOK,
                inputFilePath, outputFilePath, recipent, errorMsg);

            return decryptionOutput;
        }

        public static RegisterOutput ParseRegisterValues(RegisterInput ri)
        {
            bool readingAllOK = true;
            string errorMsg = "";

            //retrieve email and password
            string email = ri.Email;
            if (string.IsNullOrEmpty(email))
            {
                readingAllOK = false;
                errorMsg += "Błędny email.\n";

            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    readingAllOK = (addr.Address == email);

                }
                catch
                {
                    readingAllOK = false;
                    errorMsg += "Błędny email.\n";

                }
            }


            string password = ri.Password;

            if (!UsersManagement.PasswordCorrect(password))
            {
                readingAllOK = false;
                errorMsg += "Błędne hasło. \n" +
                    "Ograniczenia hasła: Minimalna długość osiem znaków; \n" +
                    "co najmniej: jedna cyfra, jedna litera, jeden znak specjalny\n";

            }

            RegisterOutput ro = new RegisterOutput(readingAllOK,
                email, password, errorMsg);
            return ro;
        }

    }
}
