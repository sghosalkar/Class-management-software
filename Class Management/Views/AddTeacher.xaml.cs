using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Data;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for AddTeacher.xaml
    /// </summary>
    public partial class AddTeacher : UserControl
    {
        public AddTeacher()
        {
            InitializeComponent();
            FillSubjects();
            FillDataGrid();
        }

        string updateTeacher = null;

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

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                if (teacher_name.Text == "" || teacher_contact_no.Text == "" || teacher_subject.Text == "" || teacher_code.Text == "")
                {
                    string msg = "Enter Teacher Name, Contact Number, Subject and Teacher Code (Mandatory)";
                    ErrorDialog(msg);
                    return;
                }
                Int64 temp;
                if (!(Int64.TryParse(teacher_contact_no.Text, out temp)))
                {
                    ErrorDialog("Enter proper contact number");
                    return;
                }
                string sql;
                if (updateTeacher == null)
                {
                    sql = "INSERT INTO teacher( teacher_name, teacher_contact_no, qualification, other_details, teacher_email, teacher_code, teacher_subject, timing_optional) VALUES('"
                                + teacher_name.Text + "', '"
                                + teacher_contact_no.Text + "', '"
                                + qualification.Text + "', '"
                                + other_details.Text + "', '"
                                + teacher_email.Text + "', '"
                                + teacher_code.Text + "', '"
                                + teacher_subject.Text + "', '"
                                + timing_optional.Text + "');";
                }
                else
                {
                    sql = "update teacher set teacher_name='" + teacher_name.Text + "', teacher_contact_no='"
                                    + teacher_contact_no.Text + "', qualification= '"
                                    + qualification.Text + "', other_details= '"
                                    + other_details.Text + "', teacher_email='"
                                    + teacher_email.Text + "', teacher_code='"
                                    + teacher_code.Text + "', teacher_subject='"
                                    + teacher_subject.Text + "', timing_optional='"
                                    + timing_optional.Text + "' where teacher_code='" + updateTeacher + "';";
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                teacher_name.Text = teacher_contact_no.Text = qualification.Text = other_details.Text = "";
                teacher_email.Text = teacher_code.Text = teacher_subject.Text = "";
                timing_optional.SelectedItem = timing_optional.Items[0];
                conn.Close();
                FillDataGrid();
                if (updateTeacher == null)
                {
                    ErrorDialog("Saved");
                }
                else
                {
                    ViewTeacher vwteach = new ViewTeacher();
                    vwteach.FillDataGrid();
                    (this.Parent as Grid).Children.Add(vwteach);
                    (this.Parent as Grid).Children.Remove(this);
                }
            }
            catch (SQLiteException eg)
            {
                ErrorDialog(eg.Message);
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        public void stringandmode(string trcode)
        {
            updateTeacher = trcode;
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = ("select * from teacher where teacher_code='" + updateTeacher + "'; ");
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    teacher_name.Text = dr.GetString(0);
                    teacher_contact_no.Text = dr.GetString(1);
                    qualification.Text = dr.GetString(2);
                    other_details.Text = dr.GetString(3);
                    teacher_email.Text = dr.GetString(4);
                    teacher_code.Text = dr.GetString(5);
                    teacher_subject.Text = dr.GetString(6);
                    timing_optional.Text = dr.GetString(7);
                }
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

        private void FillSubjects()
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
                    teacher_subject.Items.Add(dr["subject"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
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
                string sql = "SELECT teacher_name, teacher_code, teacher_subject FROM teacher;";
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
    }
}
