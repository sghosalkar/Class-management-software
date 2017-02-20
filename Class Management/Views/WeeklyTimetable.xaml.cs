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
    /// Interaction logic for WeeklyTimetable.xaml
    /// </summary>
    public partial class WeeklyTimetable : UserControl
    {
        public WeeklyTimetable()
        {
            InitializeComponent();
            
        }

        List<string> ClearList = new List<string>();
        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
            conn.Open();

            FillTeacherCode("%");
            FillDataGrid();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearList.Clear();
            conn.Close();
            conn.Dispose();
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
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

        private void searchBox_Drop(object sender, DragEventArgs e)
        {
            string data = (string)e.Data.GetData(DataFormats.StringFormat);
            e.Handled = true;
        }

        public void FillTeacherCode(string txt)
        {
            try
            {
                string sql = "SELECT teacher_code FROM teacher WHERE teacher_code LIKE '%" + txt + "%'";
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
                dr.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + " " + ex.Message + "fillteach");
            }
        }

        public void FillDataGrid()
        {
            try
            {
                string sql = "SELECT * FROM weeklytimetable;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("weeklytimetable");
                dataAdp.Fill(dt);
                manualTimetable.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
            }
            catch (Exception)
            {
                ErrorDialog("Something went wrong. Call 911");
            }
        }

        private void Cell_Drop(object sender, DragEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            string data = (string)e.Data.GetData(DataFormats.StringFormat);
            var sdr = sender as DataGridCell;
            TextBlock txtblock = sdr.Content as TextBlock;
            Tuple<DataGridCell, DataGridRow> tuple = GetDataGridRowAndCell(dep);
            DataGridRow dgr = tuple.Item2 as DataGridRow;
            DataRowView drv = dgr.Item as DataRowView;
            string row = drv["batch_name"].ToString();
            string col = sdr.Column.Header.ToString().ToLower();
            string AllLecs = "";
            string sql;
            try
            {
                sql = "SELECT " + col + " FROM weeklytimetable WHERE batch_name='" + row + "';";
                using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader dr = command1.ExecuteReader())
                    {
                        dr.Read();
                        AllLecs = dr.GetString(0);
                        if (!(AllLecs == ""))
                        {
                            AllLecs = AllLecs + ", ";
                        }
                        AllLecs = AllLecs + data;
                        dr.Close();
                    }
                    command1.Dispose();
                }
                sql = "UPDATE weeklytimetable SET " + col + "='" + AllLecs + "' WHERE batch_name='" + row + "';";
                using (SQLiteCommand command2 = new SQLiteCommand(sql, conn))
                {
                    command2.ExecuteNonQuery();
                    command2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + " " + ex.Message + "celldrop");
            }
            txtblock.Text = AllLecs;
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            e.Handled = true;
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

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = searchBox.Text;
            teachersList.Items.Clear();
            FillTeacherCode(txt);
        }
    }
}
