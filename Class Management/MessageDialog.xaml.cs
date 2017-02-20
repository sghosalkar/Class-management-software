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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)Resources["openblack"];
            Storyboard mystory2;
            mystory2 = (Storyboard)Resources["openwhite"];
            mystory.Begin(this);
            mystory2.Begin(this);
        }

        private void HideDialog(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)Resources["closeblack"];
            Storyboard mystory2;
            mystory2 = (Storyboard)Resources["closewhite"];
            mystory.Begin(this);
            mystory2.Begin(this);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
