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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS.Forms
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            show_sales();
            count_profit();
            count_stocks();
            count_transactions();
            recent_invoice();
            show_top_selling();
        }
        private void show_sales()
        {
            try
            {
                string query = "select " +
                    "sum(total) as total " +                
                    "FROM " +
                    "sales_summary  ";
                
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

           

                while (reader.Read())
                {
                    try 
                    {
                        txt_totalSales.Text = string.Format("{0:n}", ((double)reader["total"]));
                    }
                    catch
                    ( Exception ex)
                    {
                        txt_totalSales.Text = "0";
                        return;
                    }


                }
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void show_top_selling()
        {
            try
            {
                string query = "select  " +
                    "i.product_id as product_id , " +
                    " i.product_name as product_name ," +
                    " sum(s.quantity) as QTY," +
                    " sum(s.quantity * s.price) as product_total " +
                    " FROM " +
                    " inventory i" +
                    " INNER JOIN " +
                    " sales s" +
                    " WHERE" +
                    " i.product_id = s.product_id " +
                    " group by s.product_id" +
                    " order by QTY desc" +
                    " LIMIT 5";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
               
                cmd.Prepare();
                DataSet ds = new DataSet();
                DataTable dTable = new DataTable();

                MyAdapter.Fill(ds, "LoadDataBinding");
                tbl_topSelling.DataContext = ds;
                //tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void count_transactions()
        {
            try
            {
                string query = "select count(distinct invoice_num) as transaction from sales ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    txt_totalTransaction.Text = reader["transaction"].ToString();     
                }
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void count_profit()
        {
            try
            {
                string query = "select sum(ss.total) as gross, sum(s.capital*s.quantity) as capital from sales s, sales_summary ss";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    txt_totalCustomer.Text = string.Format("{0:n}", (double.Parse(reader["gross"].ToString()) - double.Parse(reader["capital"].ToString())));   
                }
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void count_stocks()
        {
            try
            {
                string query = "select sum(product_quantity) as stocks from inventory ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    txt_totalProduct.Text = reader["stocks"].ToString();
                }
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void recent_invoice()
        {
            try
            {
                string query = "select " +
                    "s.invoice_num," +
                    "s.customer_name ," +
                    "s.date_purchased ," +
                    "s. total as total " +
                    "FROM " +
                    "sales s " +
                    "group by s.invoice_num " +
                    "order by invoice_num desc " +
                    "LIMIT 5";


                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();
                DataSet ds = new DataSet();
                DataTable dTable = new DataTable();

                MyAdapter.Fill(ds, "LoadDataBinding");
                tbl_recentPurchase.DataContext = ds;
                //tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
