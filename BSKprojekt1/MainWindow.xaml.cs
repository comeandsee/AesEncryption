using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace BSKprojekt1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> users;
        public MainWindow()
        {
            InitializeComponent();
            PrepareAppDirs();
            PrepareAppUsers();
        }

        private void PrepareAppDirs()
        {
            //creates AppMainDirPath directory (if it doesn't exist) for all app files
            //and PathToPrivateKeysDir directory (if it doesn't exist) for private keys file
            Directory.CreateDirectory(Globals.AppMainDirPath);
            Directory.CreateDirectory(Globals.PathToPrivateKeysDir);

        }


        private void PrepareAppUsers()
        {
            users = UsersManagement.GetUsersListFromFile("dummy file name");
            //set listbox to display all users (as recipents of encrypted data)



            RecipentsListBox.ItemsSource = users;
            RecipentsListBoxDecryption.ItemsSource = users;
            RecipentsListBoxRegister.ItemsSource = users;
        }
        
        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SelectInputFileButtonDecryption_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                InputFileTextBoxDecryption.Text = openFileDialog.FileName;
            }
        }

    

        private bool GetSelectedValuesFromGUIEncryption(out string inputFilePath,
            out string outputFilePath, out string cipherMode, out string fileExtension, out List<User> recipents)
        {
            bool readingAllOK = true;

            //retrieve input file and output file name
            inputFilePath = InputFileTextBox.Text;
            if (string.IsNullOrEmpty(inputFilePath))
            {
                //tODO more complex error function
                Console.WriteLine("wrong input file path");
                readingAllOK = false;
            }

            //get input file extension
            fileExtension= System.IO.Path.GetExtension(inputFilePath);

            //get output file name
            string outputFileName = OutputFileTextBox.Text;
            if (string.IsNullOrEmpty(outputFileName))
            {
                Console.WriteLine("wrong out file name");
                readingAllOK = false;

            }

            if (string.IsNullOrEmpty(inputFilePath))
            {
                Console.WriteLine("wrong input file path");
                outputFilePath = "error";
                readingAllOK = false;
            }
            else
            {
                string outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
                outputFilePath = outDirectory + "\\" + outputFileName;

            }
                        
            //retrieve cipher mode
            cipherMode = cipherModeComboBox.Text;
            if (string.IsNullOrEmpty(cipherMode))
            {
                Console.WriteLine("wrong cipher mode");
                readingAllOK = false;
            }

            //retrieve selected recipents from listbox
            System.Collections.IList items = RecipentsListBox.SelectedItems;
            recipents = items.Cast<User>().ToList();
            //todo add nullcheck- now there can be a file noone can decrypt
            return readingAllOK;
        }

        private bool GetSelectedValuesFromGUIDecryption(out string inputFilePath,
            out string outputFilePath,  out User recipent)
        {
            bool readingAllOK = true;
            string outDirectory = "";
            string decodedFileName = "";
            outputFilePath = "";

            //retrieve input file and output file name
            inputFilePath = InputFileTextBoxDecryption.Text;
            if (string.IsNullOrEmpty(inputFilePath))
            {
                //tODO more complex error function
                Console.WriteLine("wrong input file path");
                readingAllOK = false;
            }
            else
            {
                outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
            }

            string outputFileName = OutputFileTextBoxDecryption.Text;
            if (string.IsNullOrEmpty(outputFileName))
            {
                Console.WriteLine("wrong out file name");
                readingAllOK = false;

            }
            else
            {
                outputFilePath = outDirectory + "\\" + outputFileName;
                decodedFileName = outDirectory + "\\result.txt";
            }
            

            //retrieve selected recipent of encoded file from listbox
            recipent = (User)RecipentsListBoxDecryption.SelectedItem;
            if(recipent == null)
            {
                readingAllOK = false;
            }
            else
            {
                Console.WriteLine("recipent " + recipent.Email);

            }

            return readingAllOK;
        }


        private bool GetSelectedValuesFromGUIRegister(out string email,
           out string password)
        {
            bool readingAllOK = true;

            //retrieve email and password
            email = TextBoxRegistrationEmail.Text;
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("wrong email");
                readingAllOK = false;
            }
            else
            {
                try
                {
                    //todo this needs polishing
                    var addr = new System.Net.Mail.MailAddress(email);
                    readingAllOK = (addr.Address == email);
                    
                }
                catch
                {
                    Console.WriteLine("wrong email");
                    readingAllOK = false;
                }
            }


            password = userPassword.Password;

            if (!UsersManagement.PasswordCorrect(password))
            {
                Console.WriteLine("wrong pswd");
                readingAllOK = false;
                //todo an explanation 'why' needed
            }
            
            return readingAllOK;
        }

        private string inputFilePath, outputFilePath, cipherMode, fileExtension;
        private List<User> recipents;

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            // progress bar config
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompletedEncryption;
            worker.DoWork += Worker_DoWorkEncryption;
            worker.ProgressChanged += Worker_ProgressChangedEncryption;
            

            //get values from the user (from gui)
            bool correctInput = GetSelectedValuesFromGUIEncryption(out inputFilePath, out outputFilePath,
              out cipherMode, out fileExtension, out recipents);
            if (correctInput)
            {
                worker.RunWorkerAsync();

            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        //TODO only for testing
        private void testbtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\Zbigniew\\Desktop\\dieta2";
            string decodedFilePath = "C:\\Users\\Zbigniew\\Desktop\\output";
            Console.WriteLine("no klikam");
            Decryption.DecryptFile(filePath, decodedFilePath,new byte[1]);

        }
        //todo end of only for testing

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            // progress bar config
           /* BackgroundWorker worker1 = new BackgroundWorker();
            worker1.RunWorkerCompleted += worker_RunWorkerCompletedDecrytpion;
            worker1.WorkerReportsProgress = true;
            worker1.DoWork += worker_DoWorkDecrytpion;     // tu trzeba zmienic zeby ten progres szedl inaczej xd
            worker1.ProgressChanged += worker_ProgressChangedDecrytpion;
            worker1.RunWorkerAsync();
            */
            string inputFilePath, outputFilePath;
            User recipent;

            bool correctInput = GetSelectedValuesFromGUIDecryption(out inputFilePath, out outputFilePath, out recipent);
            if (correctInput)
            {
                //to do DESZYFROWANIE
                //Decryption.   
            // Encryption.GenerateEncodedFile(inputFilePath, outputFilePath,
               // Globals.blockSize, cipherMode, fileExtension, recipents, worker);

            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool correctInput = GetSelectedValuesFromGUIRegister(out string email, out string password);
            if (correctInput)
            {
                UsersManagement.AddUser(email, password);
                resultTextBlockRegister.Text = "Zostałeś zarejestrowany, dziękujemy ! Tak na prawde nie xd";
            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        private void Worker_ProgressChangedEncryption(object sender, ProgressChangedEventArgs e)
        {
            EncryptionProgressBar.Value = e.ProgressPercentage;
            ProgressTextBlock.Text = Globals.statusMsgEncryption;
        }

        private void Worker_DoWorkEncryption(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            worker.ReportProgress(0);
            Encryption.GenerateEncodedFile(inputFilePath, outputFilePath,
                Globals.blockSize, cipherMode, fileExtension, recipents, worker);
                       
        }

        private void Worker_RunWorkerCompletedEncryption(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("All Done!");
            EncryptionProgressBar.Value = 0;
            ProgressTextBlock.Text = Globals.statusMsgEncryptionFinished;
        }

      

        private void worker_ProgressChangedDecrytpion(object sender, ProgressChangedEventArgs e)
        {
            DecryptionProgress.Value = e.ProgressPercentage;
            DecryptionTextBlock.Text = (string)e.UserState;
        }

        private void worker_DoWorkDecrytpion(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0, String.Format("Processing 1."));
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
                worker.ReportProgress((i + 1) * 10, String.Format("Processing  {0}.", i + 2));
            }

            worker.ReportProgress(100, "Done Processing.");
        }

        private void worker_RunWorkerCompletedDecrytpion(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("All Done!");
            DecryptionProgress.Value = 0;
            DecryptionTextBlock.Text = "";
        }

        
    }
}

