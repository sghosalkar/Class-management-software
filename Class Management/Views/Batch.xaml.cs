using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for Batch.xaml
    /// </summary>
    public partial class Batch : UserControl
    {
        public Batch()
        {
            InitializeComponent();
            FillSubjectsComboBox();
            FillTime();
            FillDataGrid();
        }

        public Batch(object context)
        {
            InitializeComponent();
            FillSubjectsComboBox();
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
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            delete_list.Clear();
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
                if (new_batch_name.Text == "" || general_timing_from == "::" || general_timing_to == "::")
                {
                    MessageBox.Show("Enter proper Batch name and Timing");
                    return;
                }
                string selected_subjects = "";
                for (int i = 0; i < batch_subjects.Items.Count; i++)
                {
                    selected_subjects += batch_subjects.Items[i].ToString() + "; ";
                }
                string sql, sql2;
                if (igotbatch == null)
                {
                    sql = "INSERT INTO batch( batch_name, general_timing_from, general_timing_to, selected_subjects) VALUES('"
                                        + new_batch_name.Text + "', '"
                                        + general_timing_from + "', '"
                                        + general_timing_to + "', '"
                                        + selected_subjects + "');";
                    sql2 = "INSERT INTO timetable VALUES('"
                                        + new_batch_name.Text + "', '', '', '', '', '', '', '');";
                }
                else
                {
                    sql = "update batch set batch_name='"
                        + new_batch_name.Text + "', general_timing_from= '"
                        + general_timing_from + "', general_timing_to ='"
                        + general_timing_to + "', selected_subjects= '"
                        + selected_subjects + "' where batch_name='"
                        + igotbatch + "';";
                    sql2 = "UPDATE timetable set batch_name='"
                        + new_batch_name.Text + "' WHERE batch_name='"
                        + igotbatch + "';";
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(sql2, conn);
                command.ExecuteNonQuery();
                conn.Close();
                new_batch_name.Text = "";
                batch_subjects.Items.Clear();
                FillDataGrid();
                if (igotbatch != null)
                {
                    igotbatch = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name);
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
                    new_batch_name.Text = dr.GetString(0);
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
                MessageBox.Show(ex.GetType().Name);
            }
        }

        private void FillSubjectsComboBox()
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
                MessageBox.Show(msg);
            }
        }
        private void FillComboBox(object sender, RoutedEventArgs e)
        {
            select_subject.Items.Clear();
            FillSubjectsComboBox();
        }

        private void FillTime()
        {
            for (int i = 1; i <= 12; i++)
            {
                from_h.Items.Add(i.ToString());
                to_h.Items.Add(i.ToString());
            }
            for (int i = 00; i < 60; i += 5)
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
                for (int i = 0; i < batch_subjects.Items.Count; i++)
                {
                    if (sub == batch_subjects.Items[i].ToString())
                    {
                        f1 = 1;
                    }
                }
                if (f1 == 0)
                {
                    batch_subjects.Items.Add(sub);
                }
            }
            catch (NullReferenceException)
            {
                label7.Visibility = Visibility.Visible;
            }
        }

        private void delete_subject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var itm = batch_subjects.SelectedItem;
                batch_subjects.Items.Remove(itm);
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                MessageBox.Show(msg);
            }
        }

        //view batch code
        List<string> delete_list = new List<string>();

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
                    string sql = "DELETE FROM batch WHERE batch_name='" + ele + "';";
                    string sql2 = "DELETE FROM timetable WHERE batch_name='" + ele + "';";
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
            }
            else
            {
                delete_rows.Visibility = Visibility.Collapsed;
                delete_all.Visibility = Visibility.Collapsed;
            }
        }

        private void delete_all_Click(object sender, RoutedEventArgs e) { }

        private void batch_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selb = (batch_list.SelectedItem as DataRowView)["batch_name"].ToString();
                stringcmode(selb);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.GetType().Name);
            }
        }
    }
}
