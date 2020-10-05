using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POS.Forms
{
    /// <summary>
    /// Interaction logic for Customers.xaml
   
    public partial class Customers : Window
    {
        DispatcherTimer _typingTimer;
        public Customers()
        {
            InitializeComponent();
            show_customers();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public int  customer_id { get;set;  }

        public String customer_address { get; set; }
        public String customer_contact { get; set; }
        public String customer_name { get; set; }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        public void show_customers()
        {
            string query = "select * from customer";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            tbl_customer.ItemsSource = dTable.DefaultView;
            connect.Close();
        }

        private void search_customer()
        {
            string query = "select * from customer " +
                            "WHERE " +
                            "customer_id LIKE @search " +
                            "OR customer_name LIKE @search " +
                            "OR customer_address LIKE @search " +
                            "OR customer_contact LIKE @search ";

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
            tbl_customer.ItemsSource = dTable.DefaultView;

            connect.Close();
        }


        private void save(object sender, RoutedEventArgs e)
        {
            if (txt_customerName.Text == "" || txt_customerContact.Text == "" || txt_customerAddress.Text == "")
            {
                MessageBox.Show("Incomplete details");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save customer details?", "Add Customer", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into customer values ('',@customer_name,@customer_address,@customer_contact) ";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@customer_name", txt_customerName.Text);
                    cmd.Parameters.AddWithValue("@customer_address", txt_customerAddress.Text);
                    cmd.Parameters.AddWithValue("@customer_contact", txt_customerContact.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Saved Data!", "Add Customer", MessageBoxButton.OK, MessageBoxImage.Information);


                    clear_details();
                    show_customers();

                    connect.Close();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        public void clear_details()
        {
            txt_customerAddress.Clear();
            txt_customerContact.Clear();
            txt_customerName.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void clear(object sender, RoutedEventArgs e)
        {
            clear_details();
        }

        private void tbl_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

            search_customer();
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
        private void delete_customer(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            int customer_id = int.Parse(dataRowView["customer_id"].ToString()) ;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you wish to delete customer details? \n Customer ID: @customer_id", "Customer", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string query = "delete from customer where customer_id = @customer_id ";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@customer_id", customer_id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully Removed Data!", "Customer", MessageBoxButton.OK, MessageBoxImage.Information);
                show_customers();
                connect.Close();
            }
            else
            {
                MessageBox.Show("Unable to Remove Data!", "Customer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }
        private void update_customer(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            customer_id = int.Parse(dataRowView["customer_id"].ToString());
            customer_address = dataRowView["customer_id"].ToString();
            customer_contact = dataRowView["customer_id"].ToString();
            customer_name = dataRowView["customer_id"].ToString();

            popup.edit_customer edit_customer = new popup.edit_customer(this);
            edit_customer.ShowDialog();


        }
    }
}
