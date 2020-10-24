using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace POS.popup
{
    /// <summary>
    /// Interaction logic for transaction_pay.xaml
    /// </summary>
    public partial class transaction_pay : Window
    {
        String dateNow = DateTime.Now.ToString("yyyy-MM-dd");

        private Forms.Transaction transaction;
        public transaction_pay(Forms.Transaction transaction1)
        {
            InitializeComponent();
            transaction = transaction1;
            txt_name.Focus();
            customer_type();
            order_items();
            order_calculation();
            order_summary();
            
        }

        class order_details
        {
            public string product_id { get; set; }
            public string product_name { get; set; }
            public int qty { get; set; }
            public double price { get; set; }
            public double total { get; set; }

            public double capital { get; set; }
        }


        private void order_summary()
        {
            lbl_invoice.Content = transaction.transaction_number.ToString();
            txt_total.Text = string.Format("{0:n}", transaction.txt_total.Content);
          

        }


        private void order_calculation()
        {

            double total_price = 0;
            foreach (Class.order_details p in tbl_orders.Items)
            {
                total_price += double.Parse(string.Format("{0:n}", p.total.ToString()));
            }
            txt_subtotal.Content = string.Format("{0:n}", total_price * 0.88);
            txt_tax.Content = string.Format("{0:n}", total_price * 0.12);
        }

        private void order_items()
        {
            foreach (Class.order_details p in transaction.tbl_orders.Items)
            {
                this.tbl_orders.Items.Add(new Class.order_details
                {
                    product_name = p.product_name,
                    product_id = p.product_id,
                    qty = p.qty,
                    total = p.total,
                    capital = p.capital,
                    price = p.price
                });
            }

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
       
            if (txt_name.Text.Equals("") || date_purchased.Text.Equals("")) { MessageBox.Show("Fill-up missing fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);  return; }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Confirm Purchase?", "Payment", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (Class.order_details p in tbl_orders.Items)
                {
                    try
                    {
                        string query = "insert into sales values " +
                            "(@product_id," +
                            "@customer_name," +
                            "@customer_address," +
                            "@customer_contact," +
                            "@customer_type," +
                            "@quantity," +
                            "@price," +
                            "@total," +
                            "@capital," +
                            "@invoice_num," +
                            "@date)";

                        String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                        MySqlConnection connect = new MySqlConnection(con);
                        connect.Open();
                        MySqlCommand cmd = new MySqlCommand(query, connect);
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@product_id", p.product_id);
                        cmd.Parameters.AddWithValue("@quantity", p.qty);
                        cmd.Parameters.AddWithValue("@price", p.price);
                        cmd.Parameters.AddWithValue("@capital", p.capital);
                        cmd.Parameters.AddWithValue("@total", double.Parse(txt_total.Text.ToString()));
                        cmd.Parameters.AddWithValue("@invoice_num", lbl_invoice.Content);
                        cmd.Parameters.AddWithValue("@date", DateTime.Parse(date_purchased.Text).ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@customer_name", txt_name.Text);
                        cmd.Parameters.AddWithValue("@customer_address", txt_address.Text);
                        cmd.Parameters.AddWithValue("@customer_contact", txt_contact.Text);
                        cmd.Parameters.AddWithValue("@customer_type", cbox_customerType.Text);
                        cmd.ExecuteNonQuery();

                        update_product(p.qty,p.product_id);                 
                        connect.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                  
                }
            }
            else
            {
                return;
            }

            sale_summary();
            //clear transaction fields if transaction is finish
            transaction.clear_transaction();
            //refresh product list from transaction
            transaction.show_products();
            //refresh transaction_num
            transaction.get_transaction_num();

            MessageBox.Show("Successfully Saved Data!", "Payment", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void sale_summary()
        {    
                        string query = "insert into sales_summary values " +                          
                            "(@invoice_num," +
                            "@total," +
                            "@date, " +
                            "@customer_type)";

                        String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                        MySqlConnection connect = new MySqlConnection(con);
                        connect.Open();
                        MySqlCommand cmd = new MySqlCommand(query, connect);
                        cmd.Prepare();          
                        cmd.Parameters.AddWithValue("@total", double.Parse(txt_total.Text.ToString()));
                        cmd.Parameters.AddWithValue("@invoice_num", lbl_invoice.Content);
                        cmd.Parameters.AddWithValue("@date", dateNow);
                        cmd.Parameters.AddWithValue("@customer_type", cbox_customerType.Text);
                        cmd.ExecuteNonQuery();
                        connect.Close();            
        }
        
        private void update_product(int qty,string id)
        {
            string query = "update inventory set product_quantity = (select product_quantity from inventory where product_id = @product_id) - @qty where product_id = @product_id";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@product_id", id);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.ExecuteNonQuery();

            connect.Close();

        }
        private void customer_type()
        {
            try
            {
                string query = "select transaction_type from customer_type";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbox_customerType.Items.Add((string)reader["transaction_type"]);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt_total_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            try {  
                if (double.Parse(txt_total.Text) <= 0)
                {
                    return;
                }
            } 
            catch (Exception ex)
            {
                txt_total.Text = transaction.txt_total.Content.ToString();
            }

        }
    }

}
