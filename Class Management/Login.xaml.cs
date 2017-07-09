using MahApps.Metro.Controls;
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

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        object context = null;
        public Login(object context)
        {
            InitializeComponent();
            this.context = context;
        }

        private void LoginFlyoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender == LoginBtn)
            {
                (context as MainWindow).LoginFlyout.IsOpen = false;
            }
            else if (sender == WindowClose)
            {
                (context as MainWindow).Close();
            }
        }

        private void LoginFlyout_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (context as MainWindow).LoginFlyout.IsOpen = false;
            }
        }
    }
}
