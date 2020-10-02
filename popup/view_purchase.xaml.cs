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
    /// Interaction logic for view_purchase.xaml
    /// </summary>
    public partial class view_purchase : Window
    {
        public view_purchase(String invoice)
        {
            InitializeComponent();
            show_details(invoice);
            order_details(invoice);
            order_calculation();
        }

        private void show_details(String invoice_num)
        {
           
            try
            {
                string query = "select customer_name , customer_address, customer_contact,transaction_type,date_purchased from sales s where invoice_num = @invoice_num ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@invoice_num", invoice_num);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lbl_date.Content = reader["date_purchased"].ToString();
                    lbl_contact.Content = reader["customer_contact"].ToString();
                    lbl_address.Content = reader["customer_address"].ToString();
                    lbl_name.Content = reader["customer_name"].ToString();
                    lbl_date.Content = reader["date_purchased"].ToString();
                    lbl_customerType.Content = reader["transaction_type"].ToString();
                }
                lbl_invoice.Content = invoice_num;
                connect.Close();
            } 
            catch (Exception ex )
            {
                return;
            }
        }
        private void order_details(String invoice_num)
        {
            try
            {
                string query = "select p.product_id as product_id,p.product_name as product_name ,s.quantity as quantity ,s.price as price,s.quantity * s.price as total from inventory p  inner join sales s where s.invoice_num = @invoice_num AND p.product_id = s.product_id";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@invoice_num", invoice_num);

                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    this.tbl_orders.Items.Add(new Class.order_details
                    {
                        product_name = reader["product_name"].ToString(),
                        product_id = reader["product_id"].ToString(),
                        qty = int.Parse(reader["quantity"].ToString()),
                        total = double.Parse(reader["total"].ToString()),
                        price = double.Parse(reader["price"].ToString())
                    });

                }
                lbl_invoice.Content = invoice_num;
                connect.Close();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void order_calculation()
        {

            double total_price = 0;
            foreach (Class.order_details p in tbl_orders.Items)
            {
                total_price += double.Parse(string.Format("{0:n}", p.total.ToString()));
            }
            lbl_total.Content = string.Format("{0:n}", total_price);
            lbl_subotal.Content = string.Format("{0:n}", total_price * 0.88);
            lbl_tax.Content = string.Format("{0:n}", total_price * 0.12);
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
