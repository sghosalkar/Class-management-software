using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for Tools.xaml
    /// </summary>
    public partial class Tools : UserControl
    {
        public Tools()
        {
            InitializeComponent();
            
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            conn.Open();
            FillSubjects();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            conn.Close();
        }

        private void change_subject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql;
                var btn = sender as Button;
                if (btn.Name == "add_subject")
                {
                    if (subject_textbox.Text == "")
                    {
                        string msg = "Enter proper Subject name.";
                        MessageBox.Show(msg);
                        return;
                    }
                    sql = "INSERT INTO subjects(subject) VALUES('" + subject_textbox.Text + "');";
                }
                else
                {
                    sql = "DELETE FROM subjects WHERE subject='" + (subject_list.SelectedItem as DataRowView)["subject"].ToString() + "';";
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command.Dispose();
                subject_textbox.Text = null;
                FillSubjects();
            }
            catch (SQLiteException)
            {
                string msg = "Check Input (The subject might already exist in the records)";
                MessageBox.Show(msg);
            }
            catch (NullReferenceException)
            {
                string msg = "Please check selection again.";
                MessageBox.Show(msg);
            }
            catch (Exception ae)
            {
                MessageBox.Show(ae.GetType().Name + " : " + ae.Message);
            }
        }

        private void FillSubjects()
        {
            string sql = "SELECT * FROM subjects;";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable("subjects");
            dataAdp.Fill(dt);
            subject_list.ItemsSource = dt.DefaultView;
            dataAdp.Update(dt);
            command.Dispose();
        }

        private void subject_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete_subject.Visibility = Visibility.Visible;
        }
    }
}
