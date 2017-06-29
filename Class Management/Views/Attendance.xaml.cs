using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
            FillBatches();
            CreateMonthColumns();
        }

        public Attendance(object context)
        {
            InitializeComponent();
            FillBatches();
            CreateMonthColumns();
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
            string batchName = (sender as Button).Content.ToString();
            //FillDataGrid((sender as Button).Content.ToString());
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

        //public void FillDataGrid(string batch_name)
        //{
        //    try
        //    {
        //        AttendanceDatagrid.Items.Clear();
        //        string sql = "SELECT student_name, reg_no FROM student WHERE batch='" + batch_name + "';";
        //        SQLiteCommand command = new SQLiteCommand(sql, conn);
        //        SQLiteDataReader dr = command.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            DataGridRow dgr = new DataGridRow()
        //            {
        //                Item = new Student() { RegNo = dr.GetString(1), Name = dr.GetString(0) },
        //                Background = Brushes.MediumSeaGreen
        //            };
        //            dgr.MouseDoubleClick += (sdr, e) =>
        //            {
        //                SwitchAttendance(sdr, e);
        //            };
        //            AttendanceDatagrid.Items.Add(dgr);
        //        }
        //        dr.Close();
        //        command.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.GetType().Name + " : " + ex.Message;
        //        ErrorDialog(msg);
        //    }
        //}

        private void SwitchAttendance(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row.Background == Brushes.LightCoral)
            {
                row.Background = Brushes.MediumSeaGreen;
            }
            else
            {
                row.Background = Brushes.LightCoral;
            }
        }

        private void SortStudentList(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Yolo");
        }

        private void CreateMonthColumns()
        {
            int year = 2017;
            int month = 6;
            int days = DateTime.DaysInMonth(year, month);

            AttendanceDataGrid.Columns.Clear();

            DataGridTextColumn RegNoColumn = new DataGridTextColumn()
            {
                Header = "Reg No",
                Binding = new Binding("RegNo")
            };
            AttendanceDataGrid.Columns.Add(RegNoColumn);
            DataGridTextColumn NameColumn = new DataGridTextColumn()
            {
                Header = "Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };
            AttendanceDataGrid.Columns.Add(NameColumn);

            for (int i = 1; i <= days; i++)
            {
                DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                dgtc.Header = i;
                FrameworkElementFactory factory1 = new FrameworkElementFactory(typeof(CheckBox));
                Binding b1 = new Binding("IsSelected");
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
                    DataGridRow dgr = new DataGridRow()
                    {
                        Item = new Student() { RegNo = dr.GetString(1), Name = dr.GetString(0) },
                        Background = Brushes.MediumSeaGreen
                    };
                    dgr.MouseDoubleClick += (sdr, e) =>
                    {
                        SwitchAttendance(sdr, e);
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
    }

    class Student
    {
        public string RegNo { get; set; }
        public string Name { get; set; }
        
    }
}
