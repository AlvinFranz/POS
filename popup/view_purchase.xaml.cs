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
        public Forms.Reports Reports;
        public view_purchase(Forms.Reports Reports1,String invoice,int check)
        {
            InitializeComponent();

            Reports = Reports1;
            change_button(check);
            show_details(invoice);
            order_details(invoice);
            order_calculation();
        }

        private void change_button(int check)
        {
            if (check == 0)
            {
                btn_delete.Visibility = Visibility.Visible;
            }
            else
            {
                btn_print.Visibility = Visibility.Visible;
            }
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
                string query = "select p.product_id as product_id,p.product_name as product_name ,s.quantity as quantity ,s.price as price,ss.total as total from inventory p,sales_summary ss,sales s where s.invoice_num = @invoice_num AND p.product_id = s.product_id AND ss.invoice_num = s.invoice_num";
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

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Delete Record?", "Sales Record", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (Class.order_details p in tbl_orders.Items)
                {
                    update_product(p.qty, p.product_id);
                }
                Reports.sort_date(DateTime.Now.ToString("MMMM-yyyy"));
                delete_sales(int.Parse(lbl_invoice.ToString()));
                MessageBox.Show("Successfully Removed Data!", "Payment", MessageBoxButton.OK, MessageBoxImage.Information);
               this.Close();

            }
            else
            {
                return;
            }
        }

        private void delete_sales(int invoice_num)
        {
            try {  
            string query = "delete from sales where invoice_num = @invoice_num; " +
                           "delete from sales_summary where invoice_num = @invoice_num;";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@invoice_num", invoice_num);
            cmd.ExecuteNonQuery();

            connect.Close();
            } 
            catch 
            {
                MessageBox.Show("Error on delete sales function line: 153");
                return;
            }

        }
        private void update_product(int qty, string id)
        {
            string query = "update inventory set product_quantity = (select product_quantity from inventory where product_id = @product_id) + @qty where product_id = @product_id";
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
    }
}
