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
            InitializeRecipentsList();
        }

        //reads recipents from users.xml file and puts them into listbox
        private void InitializeRecipentsList()
        {
            XmlNode userEmail;
            users = new List<User>();
            User user;
            
            XmlDocument doc = new XmlDocument();
            doc.Load(Globals.UsersXmlFilePath);

            XmlNode usersNode = doc.DocumentElement.
                SelectSingleNode("//" + Globals.UsersNode);

            if(usersNode == null)
            {
                Console.WriteLine("there is no users node");
                return;
            }

            //for each user
            foreach(XmlNode node in usersNode.ChildNodes)
            {
                //userEmail = node.FirstChild;
                userEmail = node[Globals.XmlEmail];
                user = new User(userEmail.InnerText);
                users.Add(user);
                Console.WriteLine("added " + user.Email);

            }

            //set listbox to display all users
            RecipentsListBox.ItemsSource = users;

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void GenerateOutputXML(String outputFileName, String algorithm, String keySize, String blockSize, String cipherMode, String iv, User[] Users)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            using (XmlWriter writer = XmlWriter.Create(outputFileName, settings))
            {
                writer.WriteStartElement(Globals.XmlMainElement);
                writer.WriteElementString(Globals.XmlAlgorithm, algorithm);
                writer.WriteElementString(Globals.XmlKeySize, keySize);
                writer.WriteElementString(Globals.XmlBlockSize, blockSize);
                writer.WriteElementString(Globals.XmlCipherMode, cipherMode);
                writer.WriteElementString(Globals.XmlIV, iv);

                writer.WriteStartElement(Globals.XmlApprovedUsers);
                foreach(User user in Users){
                    writer.WriteStartElement(Globals.XmlUser);
                    writer.WriteElementString(Globals.XmlEmail, user.Email);
                    writer.WriteElementString(Globals.XmlSessionKey, user.SessionKey);
                }
                
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = InputFileTextBox.Text;
            if (string.IsNullOrEmpty(inputFilePath)){
                //tODO more complex error function
                Console.WriteLine("wrong input file path");
                return;
            }

            string outputFileName = OutputFileTextBox.Text;
            if (string.IsNullOrEmpty(outputFileName))
            {
                Console.WriteLine("wrong out file name");
                return;
            }

            string outDirectory = System.IO.Path.GetDirectoryName(inputFilePath);
            string pathToOutFile =  outDirectory + "\\" + outputFileName;
           
            string decodedFileName = outDirectory + "\\result.txt";

            using (Aes myAes = Aes.Create())
            {
                byte[] IV;
                Encryption.EncryptToBytes(inputFilePath, pathToOutFile, myAes.Key, myAes.Mode, myAes.BlockSize, out IV);
                Encryption.DecryptStringFromBytes(pathToOutFile, decodedFileName, myAes.Key, myAes.Mode, myAes.BlockSize, IV);
            }
            resultTextBlock.Text = "operacja zakończona";

            /*String outputFile = OutputFileTextBox.Text;
            User[] Users = new User[1];
            Users[0] = new User("p@r.com", "super_secret_session_key");
            GenerateOutputXML(outputFile, "ECS", "12", "111", "modeX", "wektor", Users);
        */
        }
    }
}
