using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Data.SQLite;
using System.Data;
using System;
using System.Windows.Media;

namespace Class_Management.Views
{

    public class Week
    {
        private string _m;
        private string _t;
        private string _w;
        private string _th;
        private string _f;
        private string _s;
        private string _su;

        public string monday
        {
            get { return _m; }
            set { _m = value; }
        }

        public string tuesday
        {
            get { return _t; }
            set { _t = value; }
        }

        public string wednesday
        {
            get { return _w; }
            set { _w = value; }
        }

        public string thursday
        {
            get { return _th; }
            set { _th = value; }
        }

        public string friday
        {
            get { return _f; }
            set { _f = value; }
        }

        public string saturday
        {
            get { return _s; }
            set { _s = value; }
        }

        public string sunday
        {
            get { return _su; }
            set { _su = value; }
        }
    }

    /// <summary>
    /// Interaction logic for ManualTimetable.xaml
    /// </summary>
    public partial class ManualTimetable : UserControl
    {
        public ObservableCollection<Week> weeks = new ObservableCollection<Week>();

        public ManualTimetable()
        {
            InitializeComponent();
            FillTeacherCode();
            FillDataGrid();
            weeks.Add(new Week() { monday = "A", tuesday = "A", wednesday = "A", thursday = "A", friday = "A", saturday = "A", sunday = "A" });
            weeks.Add(new Week() { monday = "B", tuesday = "B", wednesday = "B", thursday = "B", friday = "B", saturday = "B", sunday = "B" });
            weeks.Add(new Week() { monday = "C", tuesday = "C", wednesday = "C", thursday = "C", friday = "C", saturday = "C", sunday = "C" });
            weeks.Add(new Week() { monday = "D", tuesday = "D", wednesday = "D", thursday = "D", friday = "D", saturday = "D", sunday = "D" });
            //manualTimetable.ItemsSource = weeks;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var sdr = sender as ListBoxItem;
                var data = new DataObject(DataFormats.StringFormat, sdr.Content);
                DragDrop.DoDragDrop(sdr, data, DragDropEffects.Copy);
            }
        }

        private void manualTimetable_Drop(object sender, DragEventArgs e)
        {
            
            //MessageBox.Show(sender.GetType().Name);
            string data = (string)e.Data.GetData(DataFormats.StringFormat);
            //Console.WriteLine(sender.GetType());
            //MessageBox.Show(data);
            //weeks.Add(new Week() { monday = data, tuesday = data, wednesday = data, thursday = data, friday = data, saturday = data, sunday = data });
            e.Handled = true;
        }

        private void searchBox_Drop(object sender, DragEventArgs e)
        {
            string data = (string)e.Data.GetData(DataFormats.StringFormat);
            //MessageBox.Show(data);
            e.Handled = true;
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        public void FillTeacherCode()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT teacher_code FROM teacher;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    ListBoxItem lb = new ListBoxItem();
                    lb.Content = dr["teacher_code"].ToString();
                    lb.MouseMove += Drag;
                    teachersList.Items.Add(lb);
                }
                conn.Close();
            }
            catch(Exception)
            {
                ErrorDialog("Something went wrong. Call 911");
            }
        }

        public void FillDataGrid()
        {
            try
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
                conn.Open();
                string sql = "SELECT * FROM sample;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("sample");
                dataAdp.Fill(dt);
                manualTimetable.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                /*SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    ListBoxItem lb = new ListBoxItem();
                    lb.Content = dr["teacher_code"].ToString();
                    lb.MouseMove += Drag;
                    teachersList.Items.Add(lb);
                }*/
                conn.Close();
            }
            catch (Exception)
            {
                ErrorDialog("Something went wrong. Call 911");
            }
        }

        private void manualTimetable_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void cell_drop(object sender, DragEventArgs e)
        {
            string data = (string)e.Data.GetData(DataFormats.StringFormat);
            var sdr = sender as DataGridCell;
            TextBlock cnt = sdr.Content as TextBlock;
            cnt.Text = data;
            e.Handled = true;
        }
    }
}
