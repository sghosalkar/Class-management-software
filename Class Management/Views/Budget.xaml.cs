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
    /// Interaction logic for Budget.xaml
    /// </summary>
    public partial class Budget : UserControl
    {
        public Budget()
        {
            InitializeComponent();
        }

        public Budget(object context)
        {
            InitializeComponent();
        }

        SQLiteConnection conn = new SQLiteConnection(@"Data Source=Database\MainDatabase.db;Version=3;");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard mystory;
            mystory = (Storyboard)App.Current.Resources["sb"];
            mystory.Begin(this);
            conn.Open();
            FillDataGrids();
        }

        private void closeUC_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            conn.Close();
        }

        private void ErrorDialog(string msg)
        {
            MessageDialog md = new MessageDialog();
            md.message_text.Text = msg;
            DialogSpace.Children.Add(md);
        }

        private void SaveIncomeExpense(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnName = btn.Name;
            btnName = btnName.Substring(0, (btnName.Length - 4));
            string name, transactionDate, chequeNo;
            int amount;
            try
            {
                if (btnName == "income")
                {
                    name = incomeSourceTextBox.Text;
                    transactionDate = incomeDatePicker.Text;
                    chequeNo = incomeChequeNoTextBox.Text;
                    amount = int.Parse(incomeAmountTextBox.Text);
                }
                else
                {
                    name = expenseRecipientTextBox.Text;
                    transactionDate = expenseDatePicker.Text;
                    chequeNo = expenseChequeNoTextBox.Text;
                    amount = int.Parse(expenseAmountTextBox.Text);
                }

                string sql = "INSERT INTO " + btnName + "(name, transaction_date, cheque_no, amount) VALUES('"
                    + name + "', '"
                    + transactionDate + "', '"
                    + chequeNo + "', "
                    + amount + ");";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command.Dispose();
                MessageBox.Show("Saved");
                FillDataGrids();
            }
            catch(Exception ex)
            {
                ErrorDialog("Please check all fields again");
            }
        }

        private void FillDataGrids()
        {
            FillIncomeTable();
            FillExpenseTable();
        }

        private void FillIncomeTable()
        {
            try
            {
                string sql = "SELECT * FROM income;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("income");
                dataAdp.Fill(dt);
                incomeTable.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                command.Dispose();
            }
            catch(Exception ex)
            {
                ErrorDialog(ex.GetType() + ": " + ex.Message);
            }
        }

        private void FillExpenseTable()
        {
            try
            {
                string sql = "SELECT * FROM expense;";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("expense");
                dataAdp.Fill(dt);
                expenseTable.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                command.Dispose();
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType() + ": " + ex.Message);
            }
        }

        private void DeleteIncomeExpense(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnName = btn.Name;
            btnName = btnName.Substring(0, (btnName.Length - 6));
        }
    }
}
