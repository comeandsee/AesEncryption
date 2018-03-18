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
        }
        
        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = openFileDialog.FileName;
            }
        }
        
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private bool GetSelectedValuesFromGUI(out string inputFilePath,
            out string outputFilePath, out string cipherMode, out List<User> recipents)
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

            string outputFileName = OutputFileTextBox.Text;
            if (string.IsNullOrEmpty(outputFileName))
            {
                Console.WriteLine("wrong out file name");
                readingAllOK = false;

            }

            string outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
            outputFilePath = outDirectory + "\\" + outputFileName;

            string decodedFileName = outDirectory + "\\result.txt";

            //retrieve cipher mode
            cipherMode = "ECB";
            //TODO ^

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

            string inputFilePath, outputFilePath, cipherMode;
            List<User> recipents; 

            bool correctInput = GetSelectedValuesFromGUI(out inputFilePath, out outputFilePath, out cipherMode, out recipents);
            if (correctInput)
            {
                Encryption.GenerateEncodedFile(inputFilePath, outputFilePath, Globals.blockSize, cipherMode, recipents);
                //resultTextBlock.Text = "operacja zakończona";
               
            }
            else
            {
                //TODO error message about incorrect input
            }

            
        }

        private void DoSth_Click(object sender, RoutedEventArgs e)
        {
            //EncryptionHelper.TestRSAEncrypt();
            //Encryption.GenerateSessionKey();
            //Encryption.GenerateKeyPairRSA(out string a, out string b);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

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
                worker.ReportProgress((i + 1) * 10, String.Format("Processing Iteration {0}.", i + 2));
            }

            worker.ReportProgress(100, "Done Processing.");
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("All Done!");
            EncryptionProgress.Value = 0;
            ProgressTextBlock.Text = "";
        }
    }
}

