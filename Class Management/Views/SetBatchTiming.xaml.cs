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
    /// Interaction logic for SetBatchTiming.xaml
    /// </summary>
    public partial class SetBatchTiming : UserControl
    {
        public SetBatchTiming()
        {
            InitializeComponent();
            FillBatch();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
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

        public void FillBatch()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                //MessageBox.Show("Connected");

                string sql = "SELECT batch_name, timing_from, timing_to FROM batchtiming;";
                string sql1 = "SELECT batch_name, general_timing_from, general_timing_to FROM batch;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("batchtiming");
                dataAdp.Fill(dt);
                current_timing.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                command = new SQLiteCommand(sql1, conn);
                command.ExecuteNonQuery();
                dataAdp = new SQLiteDataAdapter(command);
                dt = new DataTable("batch");
                dataAdp.Fill(dt);
                general_timing.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        string selrow;
        public void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selrow = (current_timing.SelectedItem as DataRowView)["batch_name"].ToString();
                outer.Visibility = Visibility.Visible;
                BatchName.Content = "Batch Name: " + selrow;
                //creating timepicker
                for (int i = 1, j = 0; i <= 12; i++, j += 5)
                {
                    f_h.Items.Add(i);
                    t_h.Items.Add(i);
                    f_m.Items.Add(j);
                    t_m.Items.Add(j);
                }
                f_ampm.Items.Add("AM");
                f_ampm.Items.Add("PM");
                t_ampm.Items.Add("AM");
                t_ampm.Items.Add("PM");
                //adding values from database
                try
                {
                    SQLiteConnection conn;
                    conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                    conn.Open();
                    string sql = "SELECT * FROM batchtiming WHERE batch_name='" + selrow + "'; ";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    SQLiteDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        string[] timfrmstrings = dr.GetString(1).Split(':');
                        string[] timuptostrings = dr.GetString(2).Split(':');
                        f_h.Text = timfrmstrings[0];
                        f_m.Text = timfrmstrings[1];
                        f_ampm.Text = timfrmstrings[2];
                        t_h.Text = timuptostrings[0];
                        t_m.Text = timuptostrings[1];
                        t_ampm.Text = timuptostrings[2];
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorDialog(ex.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }
        }

        private void done_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string timing_from = f_h.Text + ":" + f_m.Text + ":" + f_ampm.Text;
                string timing_to = t_h.Text + ":" + t_m.Text + ":" + t_ampm.Text;
                if (timing_from == "::" || timing_to == "::")
                {
                    ErrorDialog("Select proper Timing");
                    return;
                }
                string sql = "UPDATE batchtiming SET timing_from= '" + timing_from + "', timing_to ='" + timing_to + "' where batch_name='" + selrow + "';";
                string sql1 = "UPDATE dailytimetable1 SET batch_time='" + timing_from + "-" + timing_to + "' where batch_name='" + selrow + "';";
                string sql2 = "UPDATE dailytimetable2 SET batch_time='" + timing_from + "-" + timing_to + "' where batch_name='" + selrow + "';";
                string sql3 = "UPDATE dailytimetable3 SET batch_time='" + timing_from + "-" + timing_to + "' where batch_name='" + selrow + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(sql1, conn);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(sql2, conn);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(sql3, conn);
                command.ExecuteNonQuery();
                conn.Close();
                FillBatch();
                outer.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }
        }
    }
}
