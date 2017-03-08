using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Data.SQLite;
using System.Data;
using MahApps.Metro.Controls;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for AddBatch.xaml
    /// </summary>
    public partial class AddBatch : UserControl
    {
        public AddBatch()
        {
            InitializeComponent();
            FillComboBox();
            FillTime();
            FillDataGrid();
        }
        string igotbatch = null;
        

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

        private void save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string general_timing_from = from_h.Text + ":" + from_m.Text + ":" + from_ampm.Text;
                string general_timing_to = to_h.Text + ":" + to_m.Text + ":" + to_ampm.Text;
                if (batch_name.Text == "" || general_timing_from == "::" || general_timing_to == "::")
                {
                    ErrorDialog("Enter proper Batch name and Timing");
                    return;
                }
                string selected_subjects = "";
                for (int i = 0; i < batch_subjects.Items.Count; i++)
                {
                    selected_subjects += batch_subjects.Items[i].ToString() + "; ";
                }
                string sql, sql1, sql2, sql3, sql4;
                if (igotbatch == null)
                {
                    sql = "INSERT INTO batch( batch_name, general_timing_from, general_timing_to, selected_subjects) VALUES('"
                                        + batch_name.Text + "', '"
                                        + general_timing_from + "', '"
                                        + general_timing_to + "', '"
                                        + selected_subjects + "');";
                    sql1 = "INSERT INTO batchtiming( batch_name, timing_from, timing_to) VALUES('"
                                        + batch_name.Text + "', '"
                                        + general_timing_from + "', '"
                                        + general_timing_to + "');";
                    sql2 = "INSERT INTO dailytimetable1(batch_name, batch_time, lectures, per_lec_duration) VALUES('"
                                        + batch_name.Text + "', '" + general_timing_from + "-" + general_timing_to + "', '', '');";
                    sql3 = "INSERT INTO dailytimetable2(batch_name, batch_time, lectures, per_lec_duration) VALUES('"
                                        + batch_name.Text + "', '" + general_timing_from + "-" + general_timing_to + "', '', '');";
                    sql4 = "INSERT INTO dailytimetable3(batch_name, batch_time, lectures, per_lec_duration) VALUES('"
                                        + batch_name.Text + "', '" + general_timing_from + "-" + general_timing_to + "', '', '');";
                }
                else
                {
                    sql = "update batch set batch_name='"
                        + batch_name.Text + "', general_timing_from= '"
                        + general_timing_from + "', general_timing_to ='"
                        + general_timing_to + "', selected_subjects= '"
                        + selected_subjects + "' where batch_name='"
                        + igotbatch + "';";
                    sql1 = "update batchtiming set batch_name='"
                        + batch_name.Text + "', timing_from= '"
                        + general_timing_from + "', timing_to ='"
                        + general_timing_to + "' where batch_name='"
                        + igotbatch + "';";
                    sql2 = "UPDATE dailytimetable1 set batch_name='"
                        + batch_name.Text + "', batch_time='"
                        + general_timing_from + "-" + general_timing_to + "' WHERE batch_name='"
                        + igotbatch + "';";
                    sql3 = "UPDATE dailytimetable2 set batch_name='"
                        + batch_name.Text + "', batch_time='"
                        + general_timing_from + "-" + general_timing_to + "' WHERE batch_name='"
                        + igotbatch + "';";
                    sql4 = "UPDATE dailytimetable3 set batch_name='"
                        + batch_name.Text + "', batch_time='"
                        + general_timing_from + "-" + general_timing_to + "' WHERE batch_name='"
                        + igotbatch + "';";
                }
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
                conn.Close();
                batch_name.Text = "";
                batch_subjects.Items.Clear();
                FillDataGrid();
                if (igotbatch == null)
                {
                    ErrorDialog("Saved");
                }
                else
                {
                    ViewBatch vwbatch = new ViewBatch();
                    vwbatch.FillDataGrid();
                    (this.Parent as Grid).Children.Add(vwbatch);
                    (this.Parent as Grid).Children.Remove(this);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }            
        }

        internal void stringcmode(string batchnam)
        {
            igotbatch = batchnam;
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = ("select * from batch where batch_name='" + igotbatch + "'; ");
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    batch_name.Text = dr.GetString(0);
                    string[] timfrmstrings = dr.GetString(1).Split(':');
                    string[] timuptostrings = dr.GetString(2).Split(':');
                    string[] subjctstrings = dr.GetString(3).Split(';');

                    from_h.Text = timfrmstrings[0];
                    from_m.Text = timfrmstrings[1];
                    from_ampm.Text = timfrmstrings[2];
                    to_h.Text = timuptostrings[0];
                    to_m.Text = timuptostrings[1];
                    to_ampm.Text = timuptostrings[2];
                    Console.WriteLine("length= " + subjctstrings.Length);
                    for (int i = 0; i < subjctstrings.Length; i++)
                    {
                        if (subjctstrings[i].Trim() != "")
                            batch_subjects.Items.Add(subjctstrings[i].Trim());
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }
        }

        private void FillComboBox()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT * FROM subjects;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    select_subject.Items.Add(dr["subject"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }
        private void FillComboBox(object sender, RoutedEventArgs e)
        {
            select_subject.Items.Clear();
            FillComboBox();
        }

        private void FillTime()
        {
            for(int i = 1; i <= 12; i++)
            {
                from_h.Items.Add(i.ToString());
                to_h.Items.Add(i.ToString());
            }
            for(int i = 00; i < 60; i += 5)
            {
                from_m.Items.Add(i.ToString());
                to_m.Items.Add(i.ToString());
            }
        }

        private void add_selected_subject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int f1 = 0;
                label7.Visibility = Visibility.Collapsed;
                var sub = select_subject.SelectedItem.ToString();
                for(int i = 0; i < batch_subjects.Items.Count; i++)
                {
                    if (sub == batch_subjects.Items[i].ToString())
                    {
                        f1 = 1;
                    }
                }
                if(f1 == 0)
                {
                    batch_subjects.Items.Add(sub);
                }
            }
            catch (NullReferenceException)
            {
                label7.Visibility = Visibility.Visible;                             
            }            
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        private void delete_subject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var itm = batch_subjects.SelectedItem;
                batch_subjects.Items.Remove(itm);
            }
            catch(Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void FillDataGrid()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT batch_name FROM batch;";
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
    }
}
