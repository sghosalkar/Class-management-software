using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Data.SQLite;
using System.Data;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for ViewStudent.xaml
    /// </summary>
    public partial class ViewStudent : UserControl
    {
        public ViewStudent()
        {
            InitializeComponent();
            FillDataGrid();
            FillComboBox();
        }

        public ViewStudent(object context)
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
                string sql = "SELECT * FROM student;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("student");
                dataAdp.Fill(dt);
                student_list.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                if (chkbox.IsChecked == true)
                {
                    var dels = (student_list.SelectedItem as DataRowView)["reg_no"].ToString();
                    delete_list.Add(dels);
                    CheckDeleteBtn();
                }
                else if (chkbox.IsChecked == false)
                {
                    var dels = (student_list.SelectedItem as DataRowView)["reg_no"].ToString();
                    delete_list.Remove(dels);
                    CheckDeleteBtn();
                }
                else { }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
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
                    string sql = "DELETE FROM student WHERE reg_no='" + ele + "';";
                    string sql2 = "DELETE FROM attendance WHERE reg_no='" + ele + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand(sql2, conn);
                    command.ExecuteNonQuery();
                }
                if (f1 == 0)
                {
                    MessageBox.Show("Select row(s) to delete");
                }
                conn.Close();
                delete_list.Clear();
                CheckDeleteBtn();
                FillDataGrid();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
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

        private void Delete_all_Click(object sender, RoutedEventArgs e) { }

        private void Student_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var sels = (student_list.SelectedItem as DataRowView)["reg_no"].ToString();
                AddStudent adstud = new AddStudent();
                adstud.stringaccmode(sels);
                (this.Parent as Grid).Children.Add(adstud);
                (this.Parent as Grid).Children.Remove(this);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.GetType().Name);
            }
        }

        private void segregator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cmbbox = sender as ComboBox;
                string bname = cmbbox.SelectedItem.ToString();
                if (bname == "" || bname == "All")
                {
                    FillDataGrid();
                }
                else
                {
                    SQLiteConnection conn;
                    conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                    conn.Open();
                    string sql = "SELECT * FROM student WHERE batch='" + bname + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable("batch");
                    dataAdp.Fill(dt);
                    student_list.ItemsSource = dt.DefaultView;
                    dataAdp.Update(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void FillComboBox()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT batch_name FROM batch;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataReader dr = command.ExecuteReader();
                segregator.Items.Add("All");
                while (dr.Read())
                {
                    segregator.Items.Add(dr["batch_name"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void shortcut_Click(object sender, RoutedEventArgs e)
        {
            AddStudent viewStudent = new AddStudent();
            (this.Parent as Grid).Children.Add(viewStudent);
            (this.Parent as Grid).Children.Remove(this);
        }
    }
}
