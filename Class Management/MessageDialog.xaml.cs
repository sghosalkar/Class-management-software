using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
