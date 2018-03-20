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

            PrepareAppUsers();
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

    

        private bool GetSelectedValuesFromGUI(out string inputFilePath,
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
           
            return readingAllOK;
        }

        private bool GetSelectedValuesFromGUIDecryption(out string inputFilePath,
            out string outputFilePath,  out List<User> recipents)
        {
            bool readingAllOK = true;

            //retrieve input file and output file name
            inputFilePath = InputFileTextBoxDecryption.Text;
            if (string.IsNullOrEmpty(inputFilePath))
            {
                //tODO more complex error function
                Console.WriteLine("wrong input file path");
                readingAllOK = false;
            }

            string outputFileName = OutputFileTextBoxDecryption.Text;
            if (string.IsNullOrEmpty(outputFileName))
            {
                Console.WriteLine("wrong out file name");
                readingAllOK = false;

            }

            string outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
            outputFilePath = outDirectory + "\\" + outputFileName;

            string decodedFileName = outDirectory + "\\result.txt";

            //retrieve selected recipents from listbox
            //TODO- now it's all users
            recipents = new List<User>(users);

            return readingAllOK;
        }

        private bool GetSelectedValuesFromGUIRegister(out string email,
           out string password, out List<User> recipents)
        {
            bool readingAllOK = true;

            //retrieve input file and output file name
            email = TextBoxRegistrationEmail.Text;
            if (string.IsNullOrEmpty(email))
            {
               
                //to do lepsze sprawdzanie
                Console.WriteLine("wrong email");
                readingAllOK = false;
            }

            password = userPassword.Password;
          //  Console.WriteLine("uwaga hasło : " + password);
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("the password must contain letters ");
                readingAllOK = false;

            }

            //retrieve selected recipents from listbox
            //TODO- now it's all users
            recipents = new List<User>(users);
    
            return readingAllOK;
        }
        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            // progress bar config
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;     // tu trzeba zmienic zeby ten progres szedl inaczej xd
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();

 
            //get values from the user (from gui)
            bool correctInput = GetSelectedValuesFromGUI(out string inputFilePath, out string outputFilePath, 
              out string fileExtension, out string cipherMode, out List<User> recipents);
            if (correctInput)
            {
                Encryption.GenerateEncodedFile(inputFilePath, outputFilePath, Globals.blockSize, cipherMode, fileExtension, recipents);
                resultTextBlock.Text = "operacja zakończona";
               
            }
            else
            {
                //TODO error message about incorrect input
            }

            
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            // progress bar config
            BackgroundWorker worker1 = new BackgroundWorker();
            worker1.RunWorkerCompleted += worker_RunWorkerCompletedDecrytpion;
            worker1.WorkerReportsProgress = true;
            worker1.DoWork += worker_DoWorkDecrytpion;     // tu trzeba zmienic zeby ten progres szedl inaczej xd
            worker1.ProgressChanged += worker_ProgressChangedDecrytpion;
            worker1.RunWorkerAsync();

            string inputFilePath, outputFilePath;
            List<User> recipents;

            bool correctInput = GetSelectedValuesFromGUIDecryption(out inputFilePath, out outputFilePath, out recipents);
            if (correctInput)
            {
                //to do DESZYFROWANIE
                resultTextBlockDecryption.Text = "to jeszcze do zrobienia bardzo";
            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email, password;
            List<User> recipents;
          //  password = (sender as PasswordBox).Password;
            bool correctInput = GetSelectedValuesFromGUIRegister(out email, out password, out recipents);
            if (correctInput)
            {
                //to do 

                UsersManagement.AddUser(email, password);
                PrepareAppUsers();
                resultTextBlockRegister.Text = "Zostałeś zarejestrowany, dziękujemy ! Tak na prawde nie xd";
            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            EncryptionProgress.Value = e.ProgressPercentage;
            ProgressTextBlock.Text = (string)e.UserState;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
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

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("All Done!");
            EncryptionProgress.Value = 0;
            ProgressTextBlock.Text = "";
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

        private void Test_button_Click(object sender, RoutedEventArgs e)
        {
            
            bool correctInput = GetSelectedValuesFromGUI(out string inputFilePath, 
                out string outputFilePath, out string cipherMode, out string fileExtension, out List<User> recipents);

            int keySizeBits = 128;

            byte[] sessionKey = EncryptionHelper.GenerateSessionKey(keySizeBits);

            Dictionary<string, string> recipentsKeysDict = Encryption.GetRecipentsEncryptedSessionKeys(sessionKey, recipents);

            string tempFileWithHeader = "tempHeader.xml";
            XmlHelpers.GenerateXMLHeader(tempFileWithHeader, Globals.Algorithm,
                "123", "123", "EEE", "eee", recipentsKeysDict,fileExtension);

        }
    }
}

