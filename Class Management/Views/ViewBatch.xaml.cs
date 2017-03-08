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
    /// Interaction logic for ViewBatch.xaml
    /// </summary>
    public partial class ViewBatch : UserControl
    {
        public ViewBatch()
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

        public void FillDataGrid()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT * FROM batch;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("batch");
                dataAdp.Fill(dt);
                batch_list.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                if (chkbox.IsChecked == true)
                {
                    var delb = (batch_list.SelectedItem as DataRowView)["batch_name"].ToString();
                    delete_list.Add(delb);
                    CheckDeleteBtn();
                }
                else if (chkbox.IsChecked == false)
                {
                    var delb = (batch_list.SelectedItem as DataRowView)["batch_name"].ToString();
                    delete_list.Remove(delb);
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
                    string sql = "DELETE FROM batch WHERE batch_name='" + ele + "';";
                    string sql1 = "DELETE FROM batchtiming WHERE batch_name='" + ele + "';";
                    string sql2 = "DELETE FROM dailytimetable1 WHERE batch_name='" + ele + "';";
                    string sql3 = "DELETE FROM dailytimetable2 WHERE batch_name='" + ele + "';";
                    string sql4 = "DELETE FROM dailytimetable3 WHERE batch_name='" + ele + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand(sql1, conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand(sql2, conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand(sql3, conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand(sql4, conn);
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

        private void delete_all_Click(object sender, RoutedEventArgs e){ }

        private void batch_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selb = (batch_list.SelectedItem as DataRowView)["batch_name"].ToString();
                AddBatch adbatch = new AddBatch();
                adbatch.stringcmode(selb);
                (this.Parent as Grid).Children.Add(adbatch);
                (this.Parent as Grid).Children.Remove(this);
            }
            catch (Exception)
            {
                //ErrorDialog(ex.GetType().Name);
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
                    string sql = "SELECT * FROM batch WHERE batch_name='" + bname + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable("batch");
                    dataAdp.Fill(dt);
                    batch_list.ItemsSource = dt.DefaultView;
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
                ErrorDialog(msg);
            }
        }

    }
}
