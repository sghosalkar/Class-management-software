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
    /// Interaction logic for Attendance.xaml
    /// </summary>
    public partial class Attendance : UserControl
    {
        public Attendance()
        {
            InitializeComponent();
            FillBatches();
        }

        public Attendance(object context)
        {
            InitializeComponent();
            FillBatches();
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
            conn.Open();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            conn.Close();
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

        private void FillBatches()
        {
            Button btn1 = new Button();
            btn1.Style = Resources["BatchButton"] as Style;
            btn1.Content = "A";
            batch_list.Children.Add(btn1);
            Button btn2 = new Button();
            btn2.Style = Resources["BatchButton"] as Style;
            btn2.Content = "B";
            batch_list.Children.Add(btn2);
            Button btn3 = new Button();
            btn3.Style = Resources["BatchButton"] as Style;
            btn3.Content = "C";
            batch_list.Children.Add(btn3);
            for (int i = 0; i < 20; i++)
            {
                Button btn = new Button();
                btn.Style = Resources["BatchButton"] as Style;
                btn.Content = i;
                batch_list.Children.Add(btn);
            }
        }

        private void Button_Batch_Select_Click(object sender, RoutedEventArgs e)
        {
            FillDataGrid((sender as Button).Content.ToString());
        }

        private void Button_Batch_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = Brushes.MediumSeaGreen;
        }

        private void Button_Batch_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = Brushes.Teal;
        }

        public void FillDataGrid(string batch_name)
        {
            try
            {
                AttendanceDatagrid.Items.Clear();
                string sql = "SELECT student_name, reg_no FROM student WHERE batch='" + batch_name + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                //command.ExecuteNonQuery();
                //SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                //DataTable dt = new DataTable("student");
                //dataAdp.Fill(dt);
                //attendanceDatagrid.ItemsSource = dt.DefaultView;
                //dataAdp.Update(dt);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    AttendanceDatagrid.Items.Add(new Student() { RegNo = dr.GetString(1), Name = dr.GetString(0) });
                }
                dr.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        //private void Chk_Checked(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var chkbox = sender as CheckBox;
        //        if (chkbox.IsChecked == true)
        //        {
        //            var dels = (attendanceDatagrid.SelectedItem as DataRowView)["reg_no"].ToString();
        //            MessageBox.Show(dels);
        //        }
        //        else if (chkbox.IsChecked == false)
        //        {
        //            var dels = (attendanceDatagrid.SelectedItem as DataRowView)["reg_no"].ToString();
        //            MessageBox.Show(dels);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.GetType().Name + " : " + ex.Message;
        //        ErrorDialog(msg);
        //    }
        //}
    }

    class Student
    {
        public string RegNo { get; set; }
        public string Name { get; set; }
        
    }
}
