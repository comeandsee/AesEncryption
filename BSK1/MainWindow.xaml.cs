using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;

namespace BSK1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> users;
        private string inputFilePath, outputFilePath, cipherMode, fileExtension;
        private List<User> recipents;
        private User recipent;
        private string obtainedPassword;

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

            Console.WriteLine(Globals.AppMainDirPath);
        }

        private void PrepareAppUsers()
        {
            users = UsersManagement.GetUsersListFromFile();
           
            //set listbox to display all users (as potential recipents of encrypted data)
            RecipentsListBox.ItemsSource = users;
            RecipentsListBoxDecryption.ItemsSource = users;
            RecipentsListViewRegister.ItemsSource = users;
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
            fileExtension = System.IO.Path.GetExtension(inputFilePath);

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
            out string outputFilePath, out User recipent)
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
            if (recipent == null)
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

       
        //todo end of only for testing

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            

            //get values from the user (from gui)
            //todo setting global var recipent, inputfilepath and outputfilepath here
            bool correctInput = GetSelectedValuesFromGUIDecryption(out inputFilePath, out outputFilePath, out recipent);
            if (correctInput)
            {
                AskForRecipentPassword(recipent);

            }
            else
            {
                //TODO error message about incorrect input
            }

        }

        //opens a dialog asking for password of selected recipent
        private void AskForRecipentPassword(User recipent)
        {
            InsertPswrdWindow insertPswrdWindow = new InsertPswrdWindow(recipent.Email);

            insertPswrdWindow.Show();
            insertPswrdWindow.Owner = this;

        }

        //called by second window when user password is obtained
        //upon that decryption can be performed
        public void OnUserPasswordGiven(string password)
        {

            // progress bar config
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompletedDecryption;
            worker.DoWork += Worker_DoWorkDecryption;
            worker.ProgressChanged += Worker_ProgressChangedDecryption;

            //set obtained password
            obtainedPassword = password;
            //start decryption
            worker.RunWorkerAsync();

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool correctInput = GetSelectedValuesFromGUIRegister(out string email, out string password);
            if (correctInput)
            {
                User newUser = UsersManagement.AddUser(email, password);
                users.Add(newUser);

                //refresh all listboxes to show also newly added user
                RecipentsListBox.ItemsSource = null;
                RecipentsListBoxDecryption.ItemsSource = null;
                RecipentsListViewRegister.ItemsSource = null;

                RecipentsListBox.ItemsSource = users;
                RecipentsListBoxDecryption.ItemsSource = users;
                RecipentsListViewRegister.ItemsSource = users;

                resultTextBlockRegister.Text = "Zostałeś zarejestrowany, dziękujemy!";
            }
            else
            {
                resultTextBlockRegister.Text = "błąd rejestracji";

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

            Encryption encryption = new Encryption(inputFilePath, outputFilePath,
                Globals.blockSize, cipherMode, fileExtension, recipents);
            
            encryption.GenerateEncodedFile(worker);

        }

        private void Worker_RunWorkerCompletedEncryption(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(Globals.encryptionFinishedPopup);
            EncryptionProgressBar.Value = 0;
            ProgressTextBlock.Text = Globals.statusMsgEncryptionFinished;
        }



        private void Worker_ProgressChangedDecryption(object sender, ProgressChangedEventArgs e)
        {
            DecryptionProgressBar.Value = e.ProgressPercentage;
            DecryptionTextBlock.Text = Globals.statusMsgDecryption;
        }

        private void Worker_DoWorkDecryption(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0);
            Decryption decryption = new Decryption(inputFilePath,
                outputFilePath, recipent);
            decryption.Decrypt(worker, obtainedPassword);
               
        }

        private void Worker_RunWorkerCompletedDecryption(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(Globals.decryptionFinishedPopup);
            EncryptionProgressBar.Value = 0;
            DecryptionTextBlock.Text = Globals.statusMsgDecryptionFinished;
        }


    }
}


