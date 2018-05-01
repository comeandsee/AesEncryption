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


       

        private DecryptionOutput GetSelectedValuesFromGUIDecryption()
        {
            //enter values from input controls to EncryptionInput object
            DecryptionInput di = new DecryptionInput(InputFileTextBoxDecryption.Text,
                OutputFileTextBoxDecryption.Text,
                (User)RecipentsListBoxDecryption.SelectedItem);

            DecryptionOutput decryptionOutput = 
                InputHandlers.ParseDecryptionValues(di);

            return decryptionOutput;
        }


        private RegisterOutput GetSelectedValuesFromGUIRegister()
        {
            //enter values from input controls to EncryptionInput object
            RegisterInput ri = new RegisterInput(TextBoxRegistrationEmail.Text,
                userPassword.Password);

            RegisterOutput ro = InputHandlers.ParseRegisterValues(ri);

            return ro;
        }

        private EncryptionOutput GetSelectedValuesFromGUIEncryption()
        {
            //enter values from input controls to EncryptionInput object
            EncryptionInput ei = new EncryptionInput(InputFileTextBox.Text,
                OutputFileTextBox.Text, cipherModeComboBox.Text,
                RecipentsListBox.SelectedItems);
            
            EncryptionOutput eo = InputHandlers.ParseEncryptionValues(ei);

            return eo;
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
            EncryptionOutput eo = GetSelectedValuesFromGUIEncryption();
            bool correctInput = eo.IsInputCorrect;

            if (correctInput)
            {
                worker.RunWorkerAsync(eo);

            }
            else
            {
                MessageBoxResult result = 
                    MessageBox.Show(eo.ErrorMessage, 
                    "Błędne dane wejściowe", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
            }

        }

      
        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            //get values from the user (from gui)
            //todo setting global var here, watch out
            DecryptionOutput decryptionOutput = 
                GetSelectedValuesFromGUIDecryption();

            bool correctInput = decryptionOutput.IsInputCorrect;
            if (correctInput)
            {
                AskForRecipentPassword(decryptionOutput);

            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show(decryptionOutput.ErrorMessage,
                    "Błędne dane wejściowe", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        //opens a dialog asking for password of selected recipent
        private void AskForRecipentPassword(DecryptionOutput decryptionOutput)
        {
            InsertPswrdWindow insertPswrdWindow = 
                new InsertPswrdWindow(decryptionOutput);

            insertPswrdWindow.Show();
            insertPswrdWindow.Owner = this;

        }

        //called by second window when user password is obtained
        //upon that decryption can be performed
        public void OnUserPasswordGiven(DecryptionOutput decryptionOutput)
        {
            // progress bar config
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompletedDecryption;
            worker.DoWork += Worker_DoWorkDecryption;
            worker.ProgressChanged += Worker_ProgressChangedDecryption;
                        
            //start decryption
            worker.RunWorkerAsync(decryptionOutput);

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterOutput ro = GetSelectedValuesFromGUIRegister();
            bool correctInput = ro.IsInputCorrect;
            if (correctInput)
            {
                User newUser = UsersManagement.AddUser(ro.Email, ro.Password);
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
                MessageBoxResult result =
                    MessageBox.Show(ro.ErrorMessage,
                    "Błędne dane wejściowe", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

            EncryptionOutput eo = (EncryptionOutput)e.Argument;

            Encryption encryption = new Encryption(eo.InputFilePath, eo.OutputFilePath,
                Globals.blockSize, eo.CipherMode, eo.FileExtension, eo.Recipents);
            
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

            DecryptionOutput decryptionOutput = (DecryptionOutput)e.Argument;

            Decryption decryption = new Decryption(decryptionOutput);
            decryption.Decrypt(worker);
               
        }

        private void Worker_RunWorkerCompletedDecryption(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(Globals.decryptionFinishedPopup);
            EncryptionProgressBar.Value = 0;
            DecryptionTextBlock.Text = Globals.statusMsgDecryptionFinished;
        }


    }
}


