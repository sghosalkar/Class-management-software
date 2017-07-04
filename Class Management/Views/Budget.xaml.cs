﻿using System;
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
                string sql;
                if (igotid == null)
                {
                    sql = "INSERT INTO " + btnName + "(name, transaction_date, cheque_no, amount) VALUES('"
                        + name + "', '"
                        + transactionDate + "', '"
                        + chequeNo + "', "
                        + amount + ");";
                }
                else
                {
                    sql = "UPDATE " + btnName 
                        + " SET name='" + name 
                        + "', transaction_date='" + transactionDate 
                        + "', cheque_no='"+ chequeNo 
                        + "', amount="+ amount 
                        + " WHERE id="+ igotid + ";";
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command.Dispose();
                MessageBox.Show("Saved");
                FillDataGrids();
                if (igotid != null)
                {
                    igotid = null;
                }
            }
            catch(Exception ex)
            {
                ErrorDialog(ex.Message);
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

        List<string> deleteIncomeList = new List<string>();
        List<string> deleteExpenseList = new List<string>();

        private void Income_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                var delId = (incomeTable.SelectedItem as DataRowView)["id"].ToString();
                if (chkbox.IsChecked == true)
                {
                    deleteIncomeList.Add(delId);
                }
                else if (chkbox.IsChecked == false)
                {
                    deleteIncomeList.Remove(delId);
                }
                else { }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void Expense_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkbox = sender as CheckBox;
                var delId = (expenseTable.SelectedItem as DataRowView)["id"].ToString();
                if (chkbox.IsChecked == true)
                {
                    deleteExpenseList.Add(delId);
                }
                else if (chkbox.IsChecked == false)
                {
                    deleteExpenseList.Remove(delId);
                }
                else { }
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void DeleteIncomeExpense(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnName = btn.Name;
            btnName = btnName.Substring(0, (btnName.Length - 6));
            List<string> deleteList = new List<string>();
            if (btnName == "income") { deleteList = deleteIncomeList; }
            else { deleteList = deleteExpenseList; }
            try
            {
                if(deleteList.Count == 0)
                {
                    ErrorDialog("Select row(s) to delete");
                    return;
                }
                foreach (string ele in deleteList)
                {
                    string sql = "DELETE FROM " + btnName + " WHERE id=" + ele + ";";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
                deleteList.Clear();
                deleteIncomeList.Clear();
                deleteExpenseList.Clear();
                FillDataGrids();
            }
            catch (Exception ex)
            {
                string msg = ex.GetType().Name + " : " + ex.Message;
                ErrorDialog(msg);
            }
        }

        private void Income_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var selb = (incomeTable.SelectedItem as DataRowView)["id"].ToString();
                Stringcmode(selb, "income");
            }
            catch (Exception)
            {
                //ErrorDialog(ex.GetType().Name);
            }
        }

        private void Expense_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var selb = (expenseTable.SelectedItem as DataRowView)["id"].ToString();
                Stringcmode(selb, "expense");
            }
            catch (Exception)
            {
                //ErrorDialog(ex.GetType().Name);
            }
        }

        string igotid = null;

        internal void Stringcmode(string id, string tableName)
        {
            try
            {
                igotid = id;
                string sql = ("select * from " + tableName + " where id='" + igotid + "'; ");
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    if(tableName == "income")
                    {
                        incomeSourceTextBox.Text = dr.GetString(1);
                        incomeDatePicker.Text = dr.GetString(2);
                        incomeChequeNoTextBox.Text = dr.GetString(3);
                        incomeAmountTextBox.Text = dr.GetInt32(4).ToString();
                    }
                    else
                    {

                        expenseRecipientTextBox.Text = dr.GetString(1);
                        expenseDatePicker.Text = dr.GetString(2);
                        expenseChequeNoTextBox.Text = dr.GetString(3);
                        expenseAmountTextBox.Text = dr.GetInt32(4).ToString();
                    }
                    
                }
                dr.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.GetType().Name);
            }
        }
    }
}
