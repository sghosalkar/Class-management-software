using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Data;
using System.Data.SQLite;
using Class_Management.Views;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.89);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.89);
            LoginFlyout.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.89);
            LoginFlyout.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.89);
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;      
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);

            conn.Open();
            FillSubjects();
            FillNotification();
            FillTodaysTimetable();
            ReminderCalendar.SelectedDate = DateTime.Today.Date;
        }
        private void mainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            conn.Close();
        }

        private void ShowFlyout(int index)
        {
            var flyout = Flyouts.Items.GetItemAt(index) as Flyout;
            flyout.Theme = FlyoutTheme.Accent;
            flyout.IsOpen = true;
        }

        private void HideFlyout(int index)
        {
            var flyout = Flyouts.Items.GetItemAt(index) as Flyout;
            flyout.IsOpen = false;
        }


        //main flyout
        private void ToggleMainFlyout(object sender, RoutedEventArgs e)
        {
            if (MainFlyout.IsOpen)
            {
                CloseOpenFlyouts(sender as Button);
            }
            else
            {
                ShowFlyout(0);
                MenuIcon.Kind = PackIconMaterialKind.Backburger;
                MenuLabel.Content = "";
            }
        }


        private void ToggleSubFlyouts(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ShowFlyout(MainFlyoutPanel.Children.IndexOf(btn) + 1);
        }


        private void CloseOpenFlyouts(Button btn)
        {
            var closeMain = true;

            for (int i = 1; i < 6; i++)
            {
                if ((Flyouts.Items[i] as Flyout).IsOpen)
                {
                    HideFlyout(i);
                    closeMain = btn.Name == "" ? false : true;
                    break;
                }
            }

            if (closeMain)
            {
                MenuIcon.Kind = PackIconMaterialKind.Menu;
                MenuLabel.Content = "MENU";
                HideFlyout(0);
            }
        }


        private void HandleSubFlyouts(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            CloseOpenFlyouts(btn);
            var res = MagicallyCreateInstance(btn.Name);
            mainContent.Children.Clear();
            mainContent.Children.Add(res);
            res.Loaded += (sdr, evt) => res.BeginStoryboard(Application.Current.Resources["sb"] as Storyboard);
        }

        private void HandleTileClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnName = btn.Name;
            btnName = btnName.Substring(0, (btnName.Length - 4));
            var res = MagicallyCreateInstance(btnName);
            mainContent.Children.Clear();
            mainContent.Children.Add(res);
            res.Loaded += (sdr, evt) => res.BeginStoryboard(Application.Current.Resources["sb"] as Storyboard);
        }

        private async void ShowAbout(object sender, RoutedEventArgs e)
        {
            await this.ShowMetroDialogAsync(Resources["AboutUsDialog"] as BaseMetroDialog);
        }

        private async void HideDialog(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Name == "AboutDialogClose")
            {
                await this.HideMetroDialogAsync(Resources["AboutUsDialog"] as BaseMetroDialog);
                return;
            }
            else if(btn.Name == "ErrorDialogClose")
            {
                await this.HideMetroDialogAsync(Resources["ErrorDialog"] as BaseMetroDialog);
                return;
            }
            
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private static UserControl MagicallyCreateInstance(string className)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Console.WriteLine(className);
            var type = assembly.GetTypes().First(t => t.Name == className);
            return (UserControl)Activator.CreateInstance(type);
        }


        private void ToggleTools(object sender, RoutedEventArgs e)
        {
            if (ToolsFlyout.IsOpen == true)
            {
                ToolsFlyout.IsOpen = false;
            }
            else
            {
                ToolsFlyout.IsOpen = true;
            }
        }

        private void change_subject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql;
                var btn = sender as Button;
                if(btn.Name == "add_subject")
                {
                    if(subject_textbox.Text == "")
                    {
                        string msg = "Enter proper Subject name.";
                        ErrorDialog(msg);
                        return;
                    }
                    sql = "INSERT INTO subjects(subject) VALUES('" + subject_textbox.Text + "');";
                }
                else
                {
                    sql = "DELETE FROM subjects WHERE subject='" + (subject_list.SelectedItem as DataRowView)["subject"].ToString() + "';";
                }
                             
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                subject_textbox.Text = null;
                FillSubjects();        
            }
            catch(SQLiteException)
            {
                string msg = "Check Input (The subject might already exist in the records)";
                ErrorDialog(msg);
            }
            catch (NullReferenceException)
            {
                string msg = "Please check selection again.";
                ErrorDialog(msg);
            }
            catch (Exception ae)
            {
                MessageBox.Show(ae.GetType().Name + " : " + ae.Message);
            }
        }

        private void FillSubjects()
        {
            
            string sql = "SELECT * FROM subjects;";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable("subjects");

            dataAdp.Fill(dt);
            subject_list.ItemsSource = dt.DefaultView;

            dataAdp.Update(dt);
        }

        private void subject_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete_subject.Visibility = Visibility.Visible;
        }

        public async void ErrorDialog(string msg)
        {
            var dialog = Resources["ErrorDialog"] as BaseMetroDialog;
            await this.ShowMetroDialogAsync(dialog);
            var txtblock = dialog.FindChild<TextBlock>("message_text");
            txtblock.Text = msg;
        }

        private void LoginFlyoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender == LoginBtn)
            {
                LoginFlyout.IsOpen = false;
            }
            else if(sender == WindowClose)
            {
                this.Close();
            }
            
        }

        private void LoginFlyout_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginFlyout.IsOpen = false;
            }
        }

        private void NotificationFlyoutEvent(object sender, RoutedEventArgs e)
        {
            NotificationFlyout.IsOpen = true;
        }

        private void FillNotification()
        {
            string sql = "", temp = null;
            string[] notiList = {"subjects", "batch", "teacher", "student"};
            foreach (string ele in notiList)
            {
                sql = "SELECT * FROM " + ele + ";";
                using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader dr = command1.ExecuteReader())
                    {
                        dr.Read();
                        try {
                            temp = dr.GetString(0);
                        } catch(Exception) { }
                        if (temp == null)
                        {
                            CreateNotification(ele);
                        }
                        dr.Close();
                    }
                    command1.Dispose();
                }
            }
            CreateNotification("subjects");
            CreateNotification("batch");
            CreateNotification("teacher");
            CreateNotification("student");
        }

        private void CreateNotification(string ele)
        {
            string msg = "";
            if(ele == "subjects")
            {
                msg = "Add your first Subject";
            }
            else
            {
                msg = "Add your first " + ele.First().ToString().ToUpper() + ele.Substring(1);
            }
            Button NotiBtn = MagicallyCreateNotiButton(msg);
            NotificationStack.Children.Add(NotiBtn);
        }

        private Button MagicallyCreateNotiButton(string msg)
        {
            Button NotiBtn = new Button();
            NotiBtn.Margin = new Thickness(0);
            NotiBtn.Content = msg;
            NotiBtn.HorizontalContentAlignment = HorizontalAlignment.Left;
            NotiBtn.Style = Resources["TransparentBtn"] as Style;
            NotiBtn.FontSize = 15;
            NotiBtn.Height = 60;
            NotiBtn.Click += NotiBtn_Click;
            return NotiBtn;
        }

        private void NotiBtn_Click(object sender, RoutedEventArgs e)
        {
            Button sdr = sender as Button;
            string msg = sdr.Content.ToString().Substring(15);
            if(msg == "Subject")
            {
                ToolsFlyout.IsOpen = true;
                NotificationFlyout.IsOpen = false;
            }
            else
            {
                var res = MagicallyCreateInstance("Add" + msg);
                mainContent.Children.Clear();
                mainContent.Children.Add(res);
                NotificationFlyout.IsOpen = false;
            }
        }

        private void ReminderCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            string CurrentDate = ReminderCalendar.SelectedDate.Value.ToShortDateString();
            FillReminders(CurrentDate);
        }

        public void FillReminders(string Rdate)
        {
            string sql = "SELECT * FROM reminder WHERE remin_date='" + Rdate + "';";
            ReminderList.Items.Clear();
            using (SQLiteCommand command1 = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader dr = command1.ExecuteReader())
                {
                    string title, txt, currentDate;
                    while (dr.Read())
                    {
                        title = dr.GetString(0);
                        txt = dr.GetString(1);
                        currentDate = dr.GetString(2);
                        ListBoxItem litm = new ListBoxItem();
                        litm.Content = title;
                        litm.Style = Resources["ReminderRowStyle"] as Style;
                        litm.MouseDoubleClick += (sdr, e) =>
                        {
                            OpenReminderDialog(currentDate, title, txt);
                        };
                        ReminderList.Items.Add(litm);
                    }
                }
            }
            ReminderList.Items.Add(AddReminderBtn());
        }



        private Button AddReminderBtn()
        {
            string currentDate = ReminderCalendar.SelectedDate.Value.ToShortDateString();
            Button btn = new Button();
            btn.Style = Resources["AddReminderBtnStyle"] as Style;
            btn.Content = "Add Reminder";
            btn.Click += (sdr, e) =>
            {
                OpenReminderDialog(currentDate, null, null);
            };
            return btn;
        }

        private void OpenReminderDialog(string currentDate, string title, string txt)
        {
            ReminderDialog reminderDialog = new ReminderDialog();
            reminderDialog.SetUpdateParameters(this, currentDate, title, txt);
            DialogSpace.Children.Add(reminderDialog);
        }

        private void red_Click(object sender, RoutedEventArgs e)
        {
            var app = App.Current as App;
            string accentUrl = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + (sender as Button).Name + ".xaml";
            app.ChangeAccent(accentUrl);
        }

        public void FillTodaysTimetable()
        {
            DateTime da = DateTime.Today;
            Console.WriteLine(da.DayOfWeek.ToString().ToLower());
            string today = da.DayOfWeek.ToString().ToLower();
            string sql = "SELECT batch_name, " + today + " FROM timetable";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    string batchName, lectures;
                    while (dr.Read())
                    {
                        Console.WriteLine(dr.GetString(1));
                        batchName = dr.GetString(0);
                        lectures = dr.GetString(1);
                        ListBoxItem litm = new ListBoxItem();
                        litm.Content = "Batch " + batchName + ": " + lectures;
                        litm.Style = Resources["ReminderRowStyle"] as Style;
                        litm.HorizontalAlignment = HorizontalAlignment.Left;
                        litm.MouseDoubleClick += (sdr, e) =>
                        {
                            //yet to think
                        };
                        TodaysTimetableListBox.Items.Add(litm);
                    }
                    dr.Close();
                }
                command.Dispose();
            }
        }
    }
}
