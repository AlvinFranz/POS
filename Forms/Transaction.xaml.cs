using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POS.Forms
{
    /// <summary>
    /// Interaction logic for Transaction.xaml
    /// </summary>
    public partial class Transaction : UserControl
    {
        DispatcherTimer _typingTimer;
        public string prod_id = "";
      

        private popup.add_order order;
        public Transaction()
        {
            InitializeComponent();
            show_products();
            get_transaction_num();
        }

        public int transaction_number { get; set; }

        public void get_transaction_num()
        {
            transaction_number = 1;
            try
            {
                string query = "select invoice_num from sales order by invoice_num desc limit 1";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                //if(reader.Read().Equals(null)) { transaction_number = 1;  return; };

                while (reader.Read())
                {
                    transaction_number = int.Parse(reader["invoice_num"].ToString())+1;
                }
                //Set invoice number 
                invoice_num.Text = transaction_number.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        public void show_products()
        {
            try
            {
                string query = "select * from inventory where product_quantity != 0";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Prepare();

                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                tbl_products.ItemsSource = dTable.DefaultView;
                connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }    
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            clear_transaction();
        } 
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            popup.transaction_customer transact_customer = new popup.transaction_customer(this);
            transact_customer.ShowDialog();
        }
        private void add_item(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            prod_id = dataRowView["product_id"].ToString();
            popup.add_order order = new popup.add_order(this);
            order.ShowDialog();
        }
        private void remove_item_click(object sender, RoutedEventArgs e)
        {
            int index = tbl_orders.SelectedIndex;    
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Remove product from list?", "Order List", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                tbl_orders.Items.RemoveAt(index);                 
            }
            else
            {
                return;
            }
            order_calculation();
        }

        private void handleTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            var isbn = timer.Tag.ToString();
            search.Text = isbn;

            string query = "select * from inventory " +
                                 "WHERE " +
                                 "product_name LIKE @search " +
                                 "OR product_price LIKE @search ";

            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Parameters.AddWithValue("@search", "%" + search.Text.Trim() + "%");
            cmd.Prepare();

            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            tbl_products.ItemsSource = dTable.DefaultView;

            connect.Close();
            timer.Stop();
        }
        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_typingTimer == null)
            {

                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(600);

                _typingTimer.Tick += new EventHandler(this.handleTypingTimerTimeout);
            }
            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Tag = (sender as TextBox).Text; // This should be done with EventArgs
            _typingTimer.Start();

        }
     
        private void order_calculation()
        {

            double total_price = 0;
            foreach (Class.order_details p in tbl_orders.Items)
            {
                total_price += double.Parse(string.Format("{0:n}", p.total.ToString()));
            }
            txt_total.Content = string.Format("{0:n}", total_price);
     
        }
        private void payment_click(object sender, RoutedEventArgs e)
        {
            if (tbl_orders.Items.Count < 1 )
            {
                MessageBox.Show("No Item Selected", "Payment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

          
            popup.transaction_pay transaction = new popup.transaction_pay(this);
            transaction.ShowDialog();
        }
        public void clear_transaction()
        {
            tbl_orders.Items.Clear();
            txt_total.Content = "0.00";
            search.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Customers customer = new Customers();
            customer.ShowDialog();
        }
    }
}
