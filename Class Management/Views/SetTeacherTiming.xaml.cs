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
    /// Interaction logic for SetTeacherTiming.xaml
    /// </summary>
    public partial class SetTeacherTiming : UserControl
    {
        public SetTeacherTiming()
        {
            InitializeComponent();
            FillTeacherGrid();
            filldates();
            FillTimePicker();
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

        private void filldates()
        {
            DateTime da = DateTime.Today;
            Console.WriteLine(da.DayOfWeek + da.ToShortDateString());
            Console.WriteLine("a " + da.AddDays(1));
            switch (da.DayOfWeek.ToString())
            {
                case "Monday":
                    mon.Content = da.ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    break;

                case "Tuesday":
                    mon.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    tue.Content = da.ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    break;
                case "Wednesday":
                    mon.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    wed.Content = da.ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    break;
                case "Thursday":
                    mon.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    thu.Content = da.ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    break;
                case "Friday":
                    mon.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    fri.Content = da.ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    break;
                case "Saturday":
                    mon.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    sat.Content = da.ToString("dd-MMM-yy");
                    sun.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    break;
                case "Sunday":
                    mon.Content = da.AddDays(1).ToString("dd-MMM-yy");
                    tue.Content = da.AddDays(2).ToString("dd-MMM-yy");
                    wed.Content = da.AddDays(3).ToString("dd-MMM-yy");
                    thu.Content = da.AddDays(4).ToString("dd-MMM-yy");
                    fri.Content = da.AddDays(5).ToString("dd-MMM-yy");
                    sat.Content = da.AddDays(6).ToString("dd-MMM-yy");
                    sun.Content = da.ToString("dd-MMM-yy");
                    break;
            }
        }

        private void FillTimePicker()
        {
            try
            {
                //adding blank
                foreach (UIElement element in outer.Children)
                {
                    if (element is Grid)
                    {
                        Grid gd = (Grid)element;
                        foreach (UIElement ele in gd.Children)
                        {
                            if (ele is ComboBox)
                            {
                                ComboBox cb = (ComboBox)ele;
                                cb.Items.Add("");
                            }
                        }
                    }
                }
                //creating timepicker
                for (int i = 1, j = 0; i <= 12; i++, j += 5)
                {
                    f_h1.Items.Add(i);
                    t_h1.Items.Add(i);
                    f_m1.Items.Add(j);
                    t_m1.Items.Add(j);

                    f_h2.Items.Add(i);
                    t_h2.Items.Add(i);
                    f_m2.Items.Add(j);
                    t_m2.Items.Add(j);

                    f_h3.Items.Add(i);
                    t_h3.Items.Add(i);
                    f_m3.Items.Add(j);
                    t_m3.Items.Add(j);

                    f_h4.Items.Add(i);
                    t_h4.Items.Add(i);
                    f_m4.Items.Add(j);
                    t_m4.Items.Add(j);

                    f_h5.Items.Add(i);
                    t_h5.Items.Add(i);
                    f_m5.Items.Add(j);
                    t_m5.Items.Add(j);

                    f_h6.Items.Add(i);
                    t_h6.Items.Add(i);
                    f_m6.Items.Add(j);
                    t_m6.Items.Add(j);

                    f_h7.Items.Add(i);
                    t_h7.Items.Add(i);
                    f_m7.Items.Add(j);
                    t_m7.Items.Add(j);
                }
                f_ampm1.Items.Add("AM");
                f_ampm1.Items.Add("PM");
                t_ampm1.Items.Add("AM");
                t_ampm1.Items.Add("PM");

                f_ampm2.Items.Add("AM");
                f_ampm2.Items.Add("PM");
                t_ampm2.Items.Add("AM");
                t_ampm2.Items.Add("PM");

                f_ampm3.Items.Add("AM");
                f_ampm3.Items.Add("PM");
                t_ampm3.Items.Add("AM");
                t_ampm3.Items.Add("PM");

                f_ampm4.Items.Add("AM");
                f_ampm4.Items.Add("PM");
                t_ampm4.Items.Add("AM");
                t_ampm4.Items.Add("PM");

                f_ampm5.Items.Add("AM");
                f_ampm5.Items.Add("PM");
                t_ampm5.Items.Add("AM");
                t_ampm5.Items.Add("PM");

                f_ampm6.Items.Add("AM");
                f_ampm6.Items.Add("PM");
                t_ampm6.Items.Add("AM");
                t_ampm6.Items.Add("PM");

                f_ampm7.Items.Add("AM");
                f_ampm7.Items.Add("PM");
                t_ampm7.Items.Add("AM");
                t_ampm7.Items.Add("PM");

                //for ambu
                /*for(int i = 1; i<=7; i++)
                {
                    var combox1 = FindName("f_ampm" + i.ToString()) as ComboBox;
                    var combox2 = FindName("t_ampm" + i.ToString()) as ComboBox;
                    combox1.Items.Add("AM");
                    combox1.Items.Add("PM");
                    combox2.Items.Add("AM");
                    combox2.Items.Add("PM");
                }*/
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + ex.Message);
            }
        }

        private void FillTeacherGrid()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT * FROM teachertiming;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("teachertiming");
                dataAdp.Fill(dt);
                teacher_timing.ItemsSource = dt.DefaultView;
                default_teacher.ItemsSource = dt.DefaultView;
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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selrow = (teacher_timing.SelectedItem as DataRowView)["teacher_code"].ToString();
                outer.Visibility = Visibility.Visible;
                namelabel.Content = selrow;
                try
                {
                    SQLiteConnection conn;
                    conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                    conn.Open(); Console.WriteLine(selrow);
                    string sql = "SELECT * FROM teachertiming WHERE teacher_code='" + selrow + "'; ";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    SQLiteDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        string[] mainstr1 = dr.GetString(2).Trim().Split('-');
                        string[] timfrm1 = mainstr1[0].Trim().Split(':');
                        string[] timupto1 = mainstr1[1].Trim().Split(':');
                        f_h1.Text = timfrm1[0];
                        f_m1.Text = timfrm1[1];
                        f_ampm1.Text = timfrm1[2];
                        t_h1.Text = timupto1[0];
                        t_m1.Text = timupto1[1];
                        t_ampm1.Text = timupto1[2];

                        string[] mainstr2 = dr.GetString(3).Trim().Split('-');
                        string[] timfrm2 = mainstr2[0].Trim().Split(':');
                        string[] timupto2 = mainstr2[1].Trim().Split(':');
                        f_h2.Text = timfrm2[0];
                        f_m2.Text = timfrm2[1];
                        f_ampm2.Text = timfrm2[2];
                        t_h2.Text = timupto2[0];
                        t_m2.Text = timupto2[1];
                        t_ampm2.Text = timupto2[2];

                        string[] mainstr3 = dr.GetString(4).Trim().Split('-');
                        string[] timfrm3 = mainstr3[0].Trim().Split(':');
                        string[] timupto3 = mainstr3[1].Trim().Split(':');
                        f_h3.Text = timfrm3[0];
                        f_m3.Text = timfrm3[1];
                        f_ampm3.Text = timfrm3[2];
                        t_h3.Text = timupto3[0];
                        t_m3.Text = timupto3[1];
                        t_ampm3.Text = timupto3[2];

                        string[] mainstr4 = dr.GetString(5).Trim().Split('-');
                        string[] timfrm4 = mainstr4[0].Trim().Split(':');
                        string[] timupto4 = mainstr4[1].Trim().Split(':');
                        f_h4.Text = timfrm4[0];
                        f_m4.Text = timfrm4[1];
                        f_ampm4.Text = timfrm4[2];
                        t_h4.Text = timupto4[0];
                        t_m4.Text = timupto4[1];
                        t_ampm4.Text = timupto4[2];

                        string[] mainstr5 = dr.GetString(6).Trim().Split('-');
                        string[] timfrm5 = mainstr5[0].Trim().Split(':');
                        string[] timupto5 = mainstr5[1].Trim().Split(':');
                        f_h5.Text = timfrm5[0];
                        f_m5.Text = timfrm5[1];
                        f_ampm5.Text = timfrm5[2];
                        t_h5.Text = timupto5[0];
                        t_m5.Text = timupto5[1];
                        t_ampm5.Text = timupto5[2];

                        string[] mainstr6 = dr.GetString(7).Trim().Split('-');
                        string[] timfrm6 = mainstr6[0].Trim().Split(':');
                        string[] timupto6 = mainstr6[1].Trim().Split(':');
                        f_h6.Text = timfrm6[0];
                        f_m6.Text = timfrm6[1];
                        f_ampm6.Text = timfrm6[2];
                        t_h6.Text = timupto6[0];
                        t_m6.Text = timupto6[1];
                        t_ampm6.Text = timupto6[2];

                        string[] mainstr7 = dr.GetString(8).Trim().Split('-');
                        string[] timfrm7 = mainstr7[0].Trim().Split(':');
                        string[] timupto7 = mainstr7[1].Trim().Split(':');
                        f_h7.Text = timfrm7[0];
                        f_m7.Text = timfrm7[1];
                        f_ampm7.Text = timfrm7[2];
                        t_h7.Text = timupto7[0];
                        t_m7.Text = timupto7[1];
                        t_ampm7.Text = timupto7[2];

                        //for ambu

                        /*string[] mainstr, timfrm, timupto;
                        for(int i = 2; i<=8; i++)
                        {
                            mainstr = dr.GetString(i).Trim().Split('-');
                            timfrm = mainstr[0].Trim().Split(':');
                            timupto = mainstr[1].Trim().Split(':');
                            (FindName("f_h" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                            (FindName("f_m" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                            (FindName("f_ampm" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                            (FindName("t_h" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                            (FindName("t_m" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                            (FindName("t_ampm" + (i - 1).ToString()) as ComboBox).Text = timfrm[0];
                        }*/
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorDialog(ex.GetType().Name + ex.Message + " in inner editclick");
                }
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + ex.Message + " in outer editclick");
            }
        }

        //awesome
        private string check()
        {
            foreach (UIElement element in outer.Children)
            {
                try
                {
                    int ch1;
                    if (element is Grid)
                    {
                        Grid gd = (Grid)element;
                        ch1 = 0;
                        foreach (UIElement ele in gd.Children)
                        {
                            if (ele is ComboBox)
                            {
                                ComboBox cb = (ComboBox)ele;
                                if (cb.SelectedIndex == 0 || cb.SelectedIndex == -1)
                                {
                                    ch1++;
                                }
                            }
                        }
                        if (ch1 != 6 && ch1 != 0)
                            return gd.Name;

                    }
                }
                catch (Exception ex)
                {
                    ErrorDialog(ex.GetType().Name + ex.Message);
                }
            }
            return "";
        }

        private void done_Click(object sender, RoutedEventArgs e)
        {
            string c = check();
            if (c == "")
            {
                try
                {
                    SQLiteConnection conn1 = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                    string timing1, timing2, timing3, timing4, timing5, timing6, timing7;
                    conn1.Open();

                    if (f_h1.SelectedIndex == 0 || f_h1.SelectedIndex == -1)
                        timing1 = ":: - ::";
                    else
                        timing1 = f_h1.Text.Trim() + ":" + f_m1.Text + ":" + f_ampm1.Text + " - " + t_h1.Text + ":" + t_m1.Text + ":" + t_ampm1.Text;

                    string sql1 = "UPDATE teachertiming SET monday= '" + timing1 + "' WHERE teacher_code='" + selrow + "';";
                    SQLiteCommand command = new SQLiteCommand(sql1, conn1);
                    command.ExecuteNonQuery();


                    if (f_h2.SelectedIndex == 0 || f_h2.SelectedIndex == -1)
                        timing2 = ":: - ::";
                    else
                        timing2 = f_h2.Text.Trim() + ":" + f_m2.Text + ":" + f_ampm2.Text + " - " + t_h2.Text + ":" + t_m2.Text + ":" + t_ampm2.Text;

                    string sql2 = "UPDATE teachertiming SET tuesday= '" + timing2 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql2, conn1);
                    command.ExecuteNonQuery();


                    if (f_h3.SelectedIndex == 0 || f_h3.SelectedIndex == -1)
                        timing3 = ":: - ::";
                    else
                        timing3 = f_h3.Text.Trim() + ":" + f_m3.Text + ":" + f_ampm3.Text + " - " + t_h3.Text + ":" + t_m3.Text + ":" + t_ampm3.Text;

                    string sql3 = "UPDATE teachertiming SET wednesday= '" + timing3 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql3, conn1);
                    command.ExecuteNonQuery();


                    if (f_h4.SelectedIndex == 0 || f_h4.SelectedIndex == -1)
                        timing4 = ":: - ::";
                    else
                        timing4 = f_h4.Text.Trim() + ":" + f_m4.Text + ":" + f_ampm4.Text + " - " + t_h4.Text + ":" + t_m4.Text + ":" + t_ampm4.Text;

                    string sql4 = "UPDATE teachertiming SET thursday= '" + timing4 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql4, conn1);
                    command.ExecuteNonQuery();


                    if (f_h5.SelectedIndex == 0 || f_h5.SelectedIndex == -1)
                        timing5 = ":: - ::";
                    else
                        timing5 = f_h5.Text.Trim() + ":" + f_m5.Text + ":" + f_ampm5.Text + " - " + t_h5.Text + ":" + t_m5.Text + ":" + t_ampm5.Text;

                    string sql5 = "UPDATE teachertiming SET friday= '" + timing5 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql5, conn1);
                    command.ExecuteNonQuery();


                    if (f_h6.SelectedIndex == 0 || f_h6.SelectedIndex == -1)
                        timing6 = ":: - ::";
                    else
                        timing6 = f_h6.Text.Trim() + ":" + f_m6.Text + ":" + f_ampm6.Text + " - " + t_h6.Text + ":" + t_m6.Text + ":" + t_ampm6.Text;

                    string sql6 = "UPDATE teachertiming SET saturday= '" + timing6 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql6, conn1);
                    command.ExecuteNonQuery();


                    if (f_h7.SelectedIndex == 0 || f_h7.SelectedIndex == -1)
                        timing7 = ":: - ::";
                    else
                        timing7 = f_h7.Text.Trim() + ":" + f_m7.Text + ":" + f_ampm7.Text + " - " + t_h7.Text + ":" + t_m7.Text + ":" + t_ampm7.Text;

                    string sql7 = "UPDATE teachertiming SET sunday= '" + timing7 + "' WHERE teacher_code='" + selrow + "';";
                    command = new SQLiteCommand(sql7, conn1);
                    command.ExecuteNonQuery();

                    /*string sql, timing;
                    for(int i = 1; i<=7; i++)
                    {
                        var f_h = (FindName("f_h"+i.ToString()) as ComboBox);
                        //...and so on.
                    }*/

                    conn1.Close();
                    FillTeacherGrid();
                    Clear();

                }
                catch (Exception ex)
                {
                    ErrorDialog(ex.GetType().Name + ex.Message);
                }
                outer.Visibility = Visibility.Collapsed;
            }
            else
            {
                switch (c.Trim())
                {
                    case "Monday": Monday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Tuesday": Tuesday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Wednesday": Wednesday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Thursday": Thursday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Friday": Friday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Saturday": Saturday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                    case "Sunday": Sunday.Focus(); ErrorDialog("Improper timings of " + c.Trim()); break;
                }
            }
        }

        private void Clear()
        {
            foreach (UIElement element in outer.Children)
            {
                if (element is Grid)
                {
                    Grid gd = (Grid)element;
                    foreach (UIElement ele in gd.Children)
                    {
                        if (ele is ComboBox)
                        {
                            ComboBox cb = (ComboBox)ele;
                            cb.SelectedIndex = 0;
                        }
                    }
                }
            }
        }
    }
}
