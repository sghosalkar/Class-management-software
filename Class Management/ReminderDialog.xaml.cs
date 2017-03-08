using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for ReminderDialog.xaml
    /// </summary>
    public partial class ReminderDialog : UserControl
    {
        public ReminderDialog()
        {
            InitializeComponent();
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        bool isUpdate = false;
        string old_title, old_text, old_date;
        object context = null;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)Resources["openblack"];
            Storyboard mystory2;
            mystory2 = (Storyboard)Resources["openwhite"];
            mystory.Begin(this);
            mystory2.Begin(this);

            conn.Open();
        }

        private void HideDialog()
        {
            Storyboard mystory;
            mystory = (Storyboard)Resources["closeblack"];
            Storyboard mystory2;
            mystory2 = (Storyboard)Resources["closewhite"];
            mystory.Begin(this);
            mystory2.Begin(this);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            HideDialog();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            (this.Parent as Grid).Children.Remove(this);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public void SetUpdateParameters(object context, string reminderDate, string title, string text) {
            this.context = context;
            titleTextbox.Text = title;
            textTextbox.Text = text;
            old_title = title;
            old_text = text;
            old_date = reminderDate;
            reminderDatePicker.SelectedDate = DateTime.Parse(reminderDate);
            if(title != null || text != null)
            {
                this.isUpdate = true;
                Delete.Visibility = Visibility.Visible;
            }
        }

        private void SaveReminder(object sender, RoutedEventArgs e)
        {
            string sql;
            string currentDate = reminderDatePicker.SelectedDate.Value.ToShortDateString();
            string title = titleTextbox.Text;
            string text = textTextbox.Text;
            if (title != "" && currentDate != "")
            {
                if (isUpdate)
                {
                    sql = "UPDATE reminder SET title='" + title + "', msg_text='" + text + "', remin_date='" + currentDate
                        + "' WHERE title='" + old_title + "' AND remin_date='" + old_date + "';";
                }
                else
                {
                    sql = "INSERT INTO reminder VALUES('" + title + "', '" + text + "', '" + currentDate + "');";

                }
            }
            else
            {
                errorLabel.Content = "Please select date and enter title";
                return;
            }
            executeCommand(sql);
            (this.context as MainWindow).FillReminders(currentDate);
            HideDialog();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            string currentDate = reminderDatePicker.SelectedDate.Value.ToShortDateString();
            string title = titleTextbox.Text;
            string text = textTextbox.Text;
            string sql = "DELETE FROM reminder WHERE title = '" + title + "' AND msg_text = '" + text + "' AND remin_date = '" + currentDate + "';";
            executeCommand(sql);
            (this.context as MainWindow).FillReminders(currentDate);
            HideDialog();
        }

        private void executeCommand(string sql)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetType().Name + ": " + ex.Message);
                }
            }
        }
    }
}
