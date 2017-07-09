using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Data;
using System.Data.SQLite;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using Class_Management.Views;

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        public MainWindow()
        {
            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.89);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.89);
            LoginFlyout.Height = this.Height;
            LoginFlyout.Width = this.Width;
            LoginFlyout.Content = new Login(this);
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);

            ToolsFlyout.Width = this.Width;
            ToolsFlyout.Content = new Tools();
            FillTodaysTimetable();
            ReminderCalendar.SelectedDate = DateTime.Today.Date;
        }

        private void mainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void ToggleMainFlyout(object sender, RoutedEventArgs e)
        {
            if (MainFlyout.IsOpen)
            {
                CloseOpenFlyouts(sender as Button);
            }
            else
            {
                ShowFlyout(0);
                //MenuIcon.Kind = PackIconMaterialKind.Backburger;
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
                //MenuIcon.Kind = PackIconMaterialKind.Menu;
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
            else if (btn.Name == "ErrorDialogClose")
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

        private UserControl MagicallyCreateInstance(string className)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Console.WriteLine(className);
            var type = assembly.GetTypes().First(t => t.Name == className);
            return (UserControl)Activator.CreateInstance(type, new object[] { this });
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

        private void NotiBtn_Click(object sender, RoutedEventArgs e) { }

        private void ReminderCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            string CurrentDate = ReminderCalendar.SelectedDate.Value.ToShortDateString();
            FillReminders(CurrentDate);
        }

        public void FillReminders(string Rdate)
        {
            conn.Open();
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
            conn.Close();
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
            conn.Open();
            DateTime da = DateTime.Today;
            Console.WriteLine(da.DayOfWeek.ToString().ToLower());
            string today = da.DayOfWeek.ToString().ToLower();
            string sql = "SELECT batch_name, " + today + " FROM timetable";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    TodaysTimetableListBox.Items.Clear();
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
            conn.Close();
        }
    }
}
