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
        public InsertPswrdWindow(string userName)
        {
            InitializeComponent();
            labelUserName.Content = userName;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).OnUserPasswordGiven(userPassword.Password.ToString());
            Close();
        }

        
    }
}
