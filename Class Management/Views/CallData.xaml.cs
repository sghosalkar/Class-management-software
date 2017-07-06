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
        }

        private void SaveCallData_Click(object sender, RoutedEventArgs e)
        {
            if(!Int64.TryParse(ContactNo.Text, out long result))
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
