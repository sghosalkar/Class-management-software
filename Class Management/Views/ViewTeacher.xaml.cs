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
using System.Data;
using System.Data.SQLite;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for ViewTeacher.xaml
    /// </summary>
    public partial class ViewTeacher : UserControl
    {
        public ViewTeacher()
        {
            InitializeComponent();
            FillDataGrid();
            FillComboBox();
        }

        List<string> delete_list = new List<string>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            delete_list.Clear();
        }

        private void closeUC_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        public void FillDataGrid()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT * FROM teacher;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("teacher");
                dataAdp.Fill(dt);
                teacher_list.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                if (chkbox.IsChecked == true)
                {
                    var delt = (teacher_list.SelectedItem as DataRowView)["teacher_code"].ToString();
                    delete_list.Add(delt);
                    CheckDeleteBtn();
                }
                else if (chkbox.IsChecked == false)
                {
                    var delt = (teacher_list.SelectedItem as DataRowView)["teacher_code"].ToString();
                    delete_list.Remove(delt);
                    CheckDeleteBtn();
                }
                else { }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void delete_rows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                int f1 = 0;
                foreach (string ele in delete_list)
                {
                    f1 = 1;
                    string sql = "DELETE FROM teacher WHERE teacher_code='" + ele + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    sql = "DELETE FROM teachertiming WHERE teacher_code='" + ele + "';";
                    command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
                if (f1 == 0)
                {
                    ErrorDialog("Select row(s) to delete");
                }
                conn.Close();
                delete_list.Clear();
                CheckDeleteBtn();
                FillDataGrid();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void CheckDeleteBtn()
        {
            int i = delete_list.Count;
            if (i > 0)
            {
                delete_rows.Visibility = Visibility.Visible;
                delete_all.Visibility = Visibility.Visible;
                segregator.Visibility = Visibility.Collapsed;
            }
            else
            {
                delete_rows.Visibility = Visibility.Collapsed;
                delete_all.Visibility = Visibility.Collapsed;
                segregator.Visibility = Visibility.Visible;
            }
        }

        private void delete_all_Click(object sender, RoutedEventArgs e){}

        private void teacher_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selt = (teacher_list.SelectedItem as DataRowView)["teacher_code"].ToString();
                AddTeacher adteach = new AddTeacher();
                adteach.stringandmode(selt);
                (this.Parent as Grid).Children.Add(adteach);
                (this.Parent as Grid).Children.Remove(this);
            }
            catch (Exception){ }
        }

        private void segregator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cmbbox = sender as ComboBox;
                string sname = cmbbox.SelectedItem.ToString();
                if (sname == "" || sname == "All")
                {
                    FillDataGrid();
                }
                else
                {
                    SQLiteConnection conn;
                    conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                    conn.Open();
                    string sql = "SELECT * FROM teacher WHERE teacher_subject='" + sname + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable("batch");
                    dataAdp.Fill(dt);
                    teacher_list.ItemsSource = dt.DefaultView;
                    dataAdp.Update(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void FillComboBox()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT subject FROM subjects;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataReader dr = command.ExecuteReader();
                segregator.Items.Add("All");
                while (dr.Read())
                {
                    segregator.Items.Add(dr["subject"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }
    }
}
