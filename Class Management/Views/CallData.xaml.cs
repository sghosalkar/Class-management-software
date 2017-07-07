using Class_Management.Utilities;
using Microsoft.Win32;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for CallData.xaml
    /// </summary>
    public partial class CallData : UserControl
    {
        public CallData()
        {
            InitializeComponent();
        }

        public CallData(object context)
        {
            InitializeComponent();
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
            conn.Open();
            FillDataGrid();
        }

        private void closeUC_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            conn.Close();
            delete_list.Clear();
        }

        private void SaveCallData_Click(object sender, RoutedEventArgs e)
        {
            long result;
            if(!Int64.TryParse(ContactNo.Text, out result))
            {
                MessageBox.Show("Enter proper contact number");
                return;
            }
            try
            {
                string sql = "INSERT INTO calldata(name, contact_no) VALUES('" + StudentName.Text + "', '" + ContactNo.Text + "');";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command.Dispose();
                FillDataGrid();
                MessageBox.Show("Saved");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillDataGrid()
        {
            try
            {
                string sql = "SELECT * FROM calldata;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("calldata");
                dataAdp.Fill(dt);
                DatabaseDataGrid.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                command.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        List<string> delete_list = new List<string>();

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                var dels = (DatabaseDataGrid.SelectedItem as DataRowView)["name"].ToString();
                dels += ":" + (DatabaseDataGrid.SelectedItem as DataRowView)["contact_no"].ToString();
                if (chkbox.IsChecked == true)
                {
                    delete_list.Add(dels);
                }
                else if (chkbox.IsChecked == false)
                {
                    delete_list.Remove(dels);
                }
                else { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Delete_rows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (delete_list.Count == 0)
                {
                    MessageBox.Show("Select row(s) to delete");
                    return;
                }
                foreach (string ele in delete_list)
                {
                    string[] data = ele.Split(':');
                    string sql = "DELETE FROM calldata WHERE name='" + data[0] + "' AND contact_no='" + data[1] + "';";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
                delete_list.Clear();
                FillDataGrid();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void ImportExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelService ExcelHelper = new ExcelService();
            DataTable dt = ExcelHelper.GetDataTable();
            if (dt != null)
            {
                ExcelDataGrid.ItemsSource = dt.DefaultView;
            }
            ExcelDataGrid.Visibility = Visibility.Visible;
        }
    }
}
