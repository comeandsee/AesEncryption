using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BSKprojekt1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            String outputFile = OutputFileTextBox.Text;
            User[] Users = new User[1];
            Users[0] = new User("p@r.com", "super_secret_session_key");
            GenerateOutputXML(outputFile, "ECS", "12", "111", "modeX", "wektor", Users);
        }
    }
}
