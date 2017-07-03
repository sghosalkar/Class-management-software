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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using Class_Management.Models;
using Newtonsoft.Json;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : UserControl
    {
        public AddStudent()
        {
            InitializeComponent();
            FillBatch();
        }

        public AddStudent(object context)
        {
            InitializeComponent();
            FillBatch();
        }

        string updateStudent = null;
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
                if (student_name.Text == "" || contact_no1.Text == "" || reg_no.Text == "" || batch.Text == "")
                {
                    ErrorDialog("Student Name, Contact number, Registration Number and Batch are mandatory.");
                    return;
                }
                Int64 temp;
                if (!(Int64.TryParse(contact_no1.Text, out temp)))
                {
                    ErrorDialog("Enter proper contact number");
                    return;
                }
                string sql, sql2;
                if (updateStudent == null)
                {
                    sql = "INSERT INTO student( student_name, contact_no1, studying_at, studying_at_name, address, student_email, parent_name, contact_no2, reg_no, batch, subjects, other_details, balance_fees) VALUES('"
                                + student_name.Text + "', '"
                                + contact_no1.Text + "', '"
                                + studying_at.Text + "', '"
                                + studying_at_name.Text + "', '"
                                + address.Text + "', '"
                                + student_email.Text + "', '"
                                + parent_name.Text + "', '"
                                + contact_no2.Text + "', '"
                                + reg_no.Text + "', '"
                                + batch.Text + "', '"
                                + subjects.Text + "', '"
                                + other_details.Text + "', '"
                                + balance_fees.Text + "');";

                    Student student = new Student();
                    student.Name = student_name.Text;
                    student.RegNo = reg_no.Text;
                    string json = JsonConvert.SerializeObject(student);
                    sql2 = "INSERT INTO attendance VALUES('" 
                        + reg_no.Text + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "', '"
                        + json + "');";
                }
                else
                {
                    sql = "update student set student_name='"
                        + student_name.Text + "',  contact_no1 ='"
                        + contact_no1.Text + "',  studying_at ='"
                        + studying_at.Text + "', studying_at_name='"
                        + studying_at_name.Text + "', address='"
                        + address.Text + "', student_email='"
                        + student_email.Text + "', parent_name= '"
                        + parent_name.Text + "', contact_no2='"
                        + contact_no2.Text + "', reg_no='"
                        + reg_no.Text + "', batch='"
                        + batch.Text + "', subjects='"
                        + subjects.Text + "', other_details= '"
                        + other_details.Text + "', balance_fees='"
                        + balance_fees.Text + "' where reg_no='" + updateStudent + "';";

                    sql2 = "UPDATE attendance SET reg_no='" + reg_no.Text + "' WHERE reg_no='" + updateStudent + "';";
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(sql2, conn);
                command.ExecuteNonQuery();
                conn.Close();
                student_name.Text = contact_no1.Text = address.Text = studying_at.Text = "";
                studying_at_name.Text = student_email.Text = parent_name.Text = contact_no2.Text = reg_no.Text = "";
                batch.Text = subjects.Text = other_details.Text = balance_fees.Text = "";
                if (updateStudent == null)
                {
                    //ErrorDialog("Saved");
                    MessageBox.Show("Saved");
                }
                else
                {
                    ViewStudent vwstud = new ViewStudent();
                    vwstud.FillDataGrid();
                    (this.Parent as Grid).Children.Add(vwstud);
                    (this.Parent as Grid).Children.Remove(this);
                }
            }
            catch (SQLiteException)
            {
                ErrorDialog("Please check input again (Reg no might already be in use)");
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        internal void stringaccmode(string studcod)
        {
            updateStudent = studcod;
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = ("select * from student where reg_no='" + updateStudent + "'; ");
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    student_name.Text = dr.GetString(0);
                    contact_no1.Text = dr.GetString(1);
                    studying_at.Text = dr.GetString(2);
                    studying_at_name.Text = dr.GetString(3);
                    address.Text = dr.GetString(4);
                    student_email.Text = dr.GetString(5);
                    parent_name.Text = dr.GetString(6);
                    contact_no2.Text = dr.GetString(7);
                    reg_no.Text = dr.GetString(8);
                    batch.Text = dr.GetString(9);
                    subjects.Text = dr.GetString(10);
                    other_details.Text = dr.GetString(11);
                    balance_fees.Text = dr.GetString(12);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        private void FillBatch()
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
                while (dr.Read())
                {
                    batch.Items.Add(dr["batch_name"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void shortcut_Click(object sender, RoutedEventArgs e)
        {
            ViewStudent viewStudent = new ViewStudent();
            (this.Parent as Grid).Children.Add(viewStudent);
            (this.Parent as Grid).Children.Remove(this);
        }
    }
}
