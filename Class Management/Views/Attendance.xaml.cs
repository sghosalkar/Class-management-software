using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            GetEverythingReady();
        }

        public Attendance(object context)
        {
            InitializeComponent();
            GetEverythingReady();
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

        private void GetEverythingReady()
        {
            FillBatches();
            CreateMonthButtons();            
            CreateMonthColumns(int.Parse(DateTime.Now.ToString("MM")), DateTime.Now.Year);
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
            string batchName = (sender as Button).Content.ToString();
            FillDataGrid(batchName);
        }

        private void Button_Batch_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = Brushes.MediumSeaGreen;
        }

        private void Button_Batch_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = Brushes.Teal;
        }

        private void CreateMonthButtons()
        {
            string[] months = new string[] {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
            foreach (string month in months)
            {
                Button btn = new Button()
                {
                    Content = month,
                    Style = Resources["MonthButtonStyle"] as Style,
                };
                MonthStackPanel.Children.Add(btn);
                MonthMenu.Items.Add(new MenuItem() { Header = month });
            }
        }

        private void CreateMonthColumns(int month, int year)
        {
            int days = DateTime.DaysInMonth(year, month);

            AttendanceDataGrid.Columns.Clear();

            DataGridTextColumn RegNoColumn = new DataGridTextColumn()
            {
                Header = "Reg No",
                Binding = new Binding("RegNo")
            };
            AttendanceDataGrid.Columns.Add(RegNoColumn);
            DataGridCheckBoxColumn NameColumn = new DataGridCheckBoxColumn()
            {
                Header = "Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            AttendanceDataGrid.Columns.Add(NameColumn);

            for (int i = 1; i <= days; i++)
            {
                DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                dgtc.Header = i;
                FrameworkElementFactory factory1 = new FrameworkElementFactory(typeof(CheckBox));
                Binding b1 = new Binding("IsSelected" + i);
                b1.Mode = BindingMode.TwoWay;
                factory1.SetValue(CheckBox.IsCheckedProperty, b1);
                factory1.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(ChkSelect_Checked));
                factory1.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(ChkSelect_Checked));
                DataTemplate cellTemplate1 = new DataTemplate();
                cellTemplate1.VisualTree = factory1;
                dgtc.CellTemplate = cellTemplate1;
                AttendanceDataGrid.Columns.Add(dgtc);
            }
        }

        private void FillDataGrid(string batchName)
        {
            try
            {
                AttendanceDataGrid.Items.Clear();
                string sql = "SELECT student_name, reg_no FROM student WHERE batch='" + batchName + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Console.WriteLine(dr.GetString(0));
                    CheckBox ck = new CheckBox();
                    ck.IsChecked = true;
                    DataGridRow dgr = new DataGridRow()
                    {
                        Item = new Student() { RegNo = dr.GetString(1), Name = dr.GetString(0) },
                    };
                    AttendanceDataGrid.Items.Add(dgr);
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

        private void ChkSelect_Checked(object sender, RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            Tuple<DataGridCell, DataGridRow> tuple = GetDataGridRowAndCell(dep);
            DataGridCell cell = tuple.Item1 as DataGridCell;
            DataGridRow dgr = tuple.Item2 as DataGridRow;
            string row = (dgr.Item as Student).RegNo;
            string col = cell.Column.Header.ToString();
            MessageBox.Show(row + " : " + col);
        }

        public static Tuple<DataGridCell, DataGridRow> GetDataGridRowAndCell(DependencyObject dep)
        {
            // iteratively traverse the visual tree
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return null;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;

                // navigate further up the tree
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                DataGridRow row = dep as DataGridRow;
                return new Tuple<DataGridCell, DataGridRow>(cell, row);
            }
            return null;
        }

        private void Month_Select_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    class Student
    {
        public string RegNo { get; set; }
        public string Name { get; set; }
        public int IsSelected1 { get; set; }
        public int IsSelected2 { get; set; }
        public int IsSelected3 { get; set; }
        public int IsSelected4 { get; set; }
        public int IsSelected5 { get; set; }
        public int IsSelected6 { get; set; }
        public int IsSelected7 { get; set; }
        public int IsSelected8 { get; set; }
        public int IsSelected9 { get; set; }
        public int IsSelected10 { get; set; }
        public int IsSelected11 { get; set; }
        public int IsSelected12 { get; set; }
        public int IsSelected13 { get; set; }
        public int IsSelected14 { get; set; }
        public int IsSelected15 { get; set; }
        public int IsSelected16 { get; set; }
        public int IsSelected17 { get; set; }
        public int IsSelected18 { get; set; }
        public int IsSelected19 { get; set; }
        public int IsSelected20 { get; set; }
        public int IsSelected21 { get; set; }
        public int IsSelected22 { get; set; }
        public int IsSelected23 { get; set; }
        public int IsSelected24 { get; set; }
        public int IsSelected25 { get; set; }
        public int IsSelected26 { get; set; }
        public int IsSelected27 { get; set; }
        public int IsSelected28 { get; set; }
        public int IsSelected29 { get; set; }
        public int IsSelected30 { get; set; }
        public int IsSelected31 { get; set; }
    }
}
