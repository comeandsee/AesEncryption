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
using System.Windows.Shapes;

namespace BSK1
{
    /// <summary>
    /// Interaction logic for InsertPswrdWindow.xaml
    /// </summary>
    public partial class InsertPswrdWindow : Window
    {
        private DecryptionOutput DecryptionOutput { get; set; }
        public InsertPswrdWindow(DecryptionOutput decryptionOutput)
        {
            InitializeComponent();
            DecryptionOutput = decryptionOutput;
            labelUserName.Content = decryptionOutput.Recipent.Email;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DecryptionOutput.ObtainedPassword = userPassword.Password.ToString();
            ((MainWindow)this.Owner).OnUserPasswordGiven(DecryptionOutput);
            Close();
        }

        
    }
}
