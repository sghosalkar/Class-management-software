using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
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
using System.Windows.Controls.Primitives;

namespace Class_Management.Views
{
    /// <summary>
    /// Interaction logic for DailyTimetable.xaml
    /// </summary>
    public partial class DailyTimetable : UserControl
    {
        public DailyTimetable()
        {
            InitializeComponent();            
        }

        string ActiveTable = "";
        List<string> ClearList = new List<string>();
        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");
               

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
            conn.Open();

            FillTeacherCode("%");
            string OpeningTable = SetTimetableDay(0);
            FillDataGrid(OpeningTable);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ActiveTable = "dailytimetable1";
            ClearList.Clear();
            conn.Close();
            conn.Dispose();
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

        public void FillDataGrid(string TableName)
        {
            ActiveTable = TableName;
            string CurrentDay;
            string sql = "SELECT * FROM " + ActiveTable;
            using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
            {
                command1.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command1);
                DataTable dt = new DataTable(ActiveTable);
                dataAdp.Fill(dt);
                dailyTimetable.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                dataAdp.Dispose();
                dt.Dispose();
                command1.Dispose();
            }
            sql = "SELECT timetable_day FROM timetablelist WHERE timetable_name='" + ActiveTable + "';";
            using (SQLiteCommand command2 = new SQLiteCommand(sql, conn))
            {
                command2.ExecuteNonQuery();
                SQLiteDataReader dr = command2.ExecuteReader();
                dr.Read();
                CurrentDay = dr.GetString(0);
                DayComboBox.Text = CurrentDay;
                SetSelectedDay.Content = "Set Timetable for " + CurrentDay;
                dr.Close();
                command2.Dispose();
            }
            FillSuggestions(CurrentDay);
            Button selbtn = FindName(ActiveTable) as Button;
            ClearTableBtn();
            selbtn.Background = Brushes.White;
            selbtn.Foreground = Brushes.DarkSlateBlue;
            try
            {
                
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + " " + ex.Message + "filldatagrid");
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
            //string col = sdr.Column.Header.ToString().ToLower();
            string AllLecs = "";
            string sql;
            try
            {
                sql = "SELECT lectures FROM " + ActiveTable + " WHERE batch_name='" + row + "';";
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
                sql = "UPDATE " + ActiveTable + " SET lectures='" + AllLecs + "' WHERE batch_name='" + row + "';";
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
            //MessageBox.Show(row + " " + col);
            SetLecDuration(AllLecs, row);
            txtblock.Text = AllLecs;
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            e.Handled = true;
        }

        private void SetLecDuration(string AllLecs, string row)
        {
            string[] lecs = AllLecs.Trim().Split(',');
            int lecno = 0;
            foreach (string lec in lecs)
            {
                lecno++;
            }
            string sql, duratn;
            string[] time;
            int hr, min;
            int timefrm, timeto;
            try
            {
                sql = "SELECT batch_time FROM " + ActiveTable + " WHERE batch_name='" + row + "';";
                using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader dr = command1.ExecuteReader())
                    {
                        dr.Read();
                        time = dr.GetString(0).Trim().Split('-');
                        dr.Close();
                    }
                    command1.Dispose();
                }
                timefrm = MinuteSpan(time[0]);
                timeto = MinuteSpan(time[1]);
                int itemp = (((timeto - timefrm) / 10) / lecno) * 10;
                int jtemp = ((timeto - timefrm) - (itemp * lecno)) / 5;
                if (jtemp >= lecno)
                {
                    itemp += 5;
                }
                hr = itemp / 60;
                min = itemp % 60;
                duratn = hr.ToString() + " hr " + min.ToString() + " min";
                sql = "UPDATE " + ActiveTable + " SET per_lec_duration='" + duratn + "' WHERE batch_name='" + row + "';";
                using (SQLiteCommand command2 = new SQLiteCommand(sql, conn))
                {
                    command2.ExecuteNonQuery();
                    command2.Dispose();
                }
                FillDataGrid(ActiveTable);
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + " " + ex.Message + "setlecduration");
            }
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
            ClearListBox();
            FillTeacherCode(txt);
        }

        private void ClearListBox()
        {
            teachersList.Items.Clear();
        }

        private void table_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ActiveTable = btn.Name;
            FillDataGrid(ActiveTable);
            //MessageBox.Show(ActiveTable);
        }

        public void ClearTableBtn()
        {
            dailytimetable1.Background = Brushes.DarkSlateBlue;
            dailytimetable1.Foreground = Brushes.White;
            dailytimetable2.Background = Brushes.DarkSlateBlue;
            dailytimetable2.Foreground = Brushes.White; 
            dailytimetable3.Background = Brushes.DarkSlateBlue;
            dailytimetable3.Foreground = Brushes.White;
        }        

        private string SetTimetableDay(int index)
        {
            DateTime tdy = DateTime.Today;
            string ActiveDay = "";
            string sql, TimetableName = "";
            string errmsg = "No current row";
            index--;
            while (errmsg == "No current row")
            {
                index++;
                try
                {
                    ActiveDay = tdy.AddDays(index).DayOfWeek.ToString();
                    sql = "SELECT timetable_name FROM timetablelist WHERE timetable_day='" + ActiveDay + "';";
                    using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader dr = command1.ExecuteReader())
                        {
                            dr.Read();
                            TimetableName = dr.GetString(0);
                            errmsg = "";
                            dr.Close();
                        }
                        command1.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                }               
            }          
            DayComboBox.Text = ActiveDay;
            return TimetableName;
        }

        private void SetSelectedDay_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(DayComboBox.Text);
            try
            {
                string sql;
                sql = "UPDATE timetablelist SET timetable_day='" + DayComboBox.Text.Trim() + "' WHERE timetable_name='" + ActiveTable + "';";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                ErrorDialog("Table" + ActiveTable.Substring(14) + " successfully set for " + DayComboBox.Text);
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name + "setselecteddayclick");
            }            
        }

        private void DayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSelectedDay.Content = "Set Timetable for " + DayComboBox.SelectedItem.ToString().Split(':').GetValue(1).ToString().Trim();
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                if (chkbox.IsChecked == true)
                {
                    var clritm = (dailyTimetable.SelectedItem as DataRowView)["batch_name"].ToString();
                    ClearList.Add(clritm);
                    CheckClearBtn();
                }
                else if (chkbox.IsChecked == false)
                {
                    var delb = (dailyTimetable.SelectedItem as DataRowView)["batch_name"].ToString();
                    ClearList.Remove(delb);
                    CheckClearBtn();
                }
                else { }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message + "chkchecked";
                ErrorDialog(msg);
            }
        }

        private void CheckClearBtn()
        {
            int i = ClearList.Count;
            if (i > 0)
            {
                Eraser.Visibility = Visibility.Visible;
                DayComboBox.Visibility = Visibility.Collapsed;
                SetSelectedDay.Visibility = Visibility.Collapsed;
            }
            else
            {
                Eraser.Visibility = Visibility.Collapsed;
                DayComboBox.Visibility = Visibility.Visible;
                SetSelectedDay.Visibility = Visibility.Visible;
            }
        }

        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql;
                foreach (string ele in ClearList)
                {
                    sql = "UPDATE " + ActiveTable + " SET lectures='', per_lec_duration='' WHERE batch_name='" + ele + "';";
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                        command.Dispose();
                    }
                }
                ClearList.Clear();
                CheckClearBtn();
                FillDataGrid(ActiveTable);
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.Message + "eraserclick");
            }           
        }

        public void FillSuggestions(string CurrentDay)
        {
            List<string> tchrs = new List<string>();
            //List<Tuple<string, string>> tr = new List<Tuple<string, string>>();
            //MessageBox.Show(CurrentDay);
            string sql = "SELECT batch_name, batch_time FROM " + ActiveTable;
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    string batch, timing;
                    while (dr.Read())
                    {
                        batch = dr.GetString(0);
                        timing = dr.GetString(1);
                        tchrs = GetMatchingTeachers(batch, timing, CurrentDay);
                        string ttl = null;
                        foreach (string ele in tchrs)
                        {
                            ttl += ele;
                        }
                        //MessageBox.Show(ttl);
                        DisplayInDataGridCell(batch, ttl);
                    }
                    dr.Close();
                }
                command.Dispose();
            }
        }

        private void DisplayInDataGridCell(string batch, string ttl)
        {
            for(int i = 0; i < dailyTimetable.Items.Count; i++)
            {
                string bth = (dailyTimetable.Items[i] as DataRowView).Row.ItemArray[0].ToString();
                if (batch == bth)
                {
                    ((TextBlock)(GetCell(i, 5).Content)).Text = ttl;

                    /*
                    var row = dailyTimetable.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    DataGridCellInfo dgci = new DataGridCellInfo(dailyTimetable.Items[i], dailyTimetable.Columns[5]);
                    dailyTimetable.SelectedCells.Clear();
                    dailyTimetable.SelectedCells.Add(dgci);
                    dailyTimetable.CurrentCell = dgci;
                    DataGridCell dcell = dailyTimetable.SelectedItem as DataGridCell;
                    DataTemplate dttem = dcell.ContentTemplate;
                    dttem.DataType = typeof(TextBlock);
                    TextBlock txt = dcell.Content as TextBlock;
                    txt.Text = ttl;
                    /*
                    var cellContent = dgci.Column.GetCellContent(dgci.Item);
                    if(cellContent != null)
                    {
                        DataGridCell dcell = cellContent.Parent as DataGridCell;
                        TextBlock txt = dcell.Content as TextBlock;
                        txt.Text = ttl;
                    }
                    
                    /*TextBlock cellContent = dailyTimetable.Columns[0].GetCellContent(row) as TextBlock;
                    cellContent.Text = ttl;*/
                }
                /*DataRowView drv = dailyTimetable.Items[i] as DataRowView;
                string str = drv["batch_name"].ToString();
                MessageBox.Show(str);*/
                /*DataGridRow row = (DataGridRow)dailyTimetable.ItemContainerGenerator.ContainerFromIndex(i);
                TextBlock cellContent = dailyTimetable.Columns[0].GetCellContent(row) as TextBlock;
                if (cellContent != null)
                {
                    //MessageBox.Show(cellContent.Text + "here bro");
                    object item = dailyTimetable.Items[i];
                    dailyTimetable.SelectedItem = item;
                    dailyTimetable.ScrollIntoView(item);
                    row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
                }*/
            }
        }

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dailyTimetable.ScrollIntoView(rowContainer, dailyTimetable.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dailyTimetable.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dailyTimetable.UpdateLayout();
                dailyTimetable.ScrollIntoView(dailyTimetable.Items[index]);
                row = (DataGridRow)dailyTimetable.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }

            }
            return child;
        }

        private List<string> GetMatchingTeachers(string batch, string timing, string CurrentDay)
        {
            //MessageBox.Show(batch + timing + CurrentDay);
            List<string> tchrs = new List<string>();
            int[] bTime = new int[2];
            int[] tTime = new int[2];
            bTime[0] = MinuteSpan(timing.Split('-').GetValue(0).ToString().Trim());
            bTime[1] = MinuteSpan(timing.Split('-').GetValue(1).ToString().Trim());

            string sql = "SELECT teacher_code, " + CurrentDay.Trim().ToLower() + " FROM teachertiming;";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    string TchrCode, TchrTiming;
                    while (dr.Read())
                    {
                        TchrCode = dr.GetString(0);
                        TchrTiming = dr.GetString(1);
                        tTime[0] = MinuteSpan(TchrTiming.Split('-').GetValue(0).ToString().Trim());
                        tTime[1] = MinuteSpan(TchrTiming.Split('-').GetValue(1).ToString().Trim());
                        if((bTime[0] < tTime[1]) && (bTime[1] > tTime[0]))
                        {
                            if (((tTime[1] - bTime[0]) >= 60) || ((bTime[1] - tTime[0]) >= 60))
                            {
                                tchrs.Add(TchrCode);
                            }                            
                        }
                    }
                    dr.Close();
                }
                command.Dispose();
            }
            return tchrs;
        }

        private int MinuteSpan(string timeBlock)
        {
            string[] timeSplit = timeBlock.Trim().Split(':');
            int minConv = int.Parse(timeSplit[0]) * 60 + int.Parse(timeSplit[1]);
            if (timeSplit[2].Trim() == "PM")
            {
                minConv += 720;
            }
            return minConv;            
        }

        
    }

}
