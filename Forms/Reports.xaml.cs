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
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        
        public Reports()
        {
            InitializeComponent();

            sort_date(DateTime.Now.ToString("MMMM-yyyy"));

        }


        public void sort_date(string date)
        {
            lbl_header.Content = date + " Monthly Report";

            //converted to get 1st day of variable date
            DateTime convert_date = DateTime.Parse(date);
     
            show_sales(convert_date);
            count_sales(convert_date);
            count_gross(convert_date);
            count_profit(convert_date);
            count_productSold(convert_date);
            count_capital(convert_date);
            count_retailer(convert_date);
            count_reseller(convert_date);
            count_trendingProduct(convert_date);
        }
 
   
        private void count_sales (DateTime date)
        {
            double count_sales = 0;
            double count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"); 

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select " +
                    " count(DISTINCT invoice_num) as count_sales, " +
                    "(select count(DISTINCT invoice_num) from sales where date_purchased BETWEEN @date4 AND @date3) as count_prev_sales " +
                    " from " +
                    " sales " +
                    " WHERE " +
                    " date_purchased BETWEEN @date2 AND @date1 ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    count_sales = double.Parse(reader["count_sales"].ToString());
                    count_prev_sales = double.Parse(reader["count_prev_sales"].ToString());
                }
                connect.Close();

                lbl_sales.Content = string.Format("{0:n}", count_sales);
                difference = count_sales - count_prev_sales;

                // if previous sales = 0 and to avoid dividing by 0
                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales) * 100;
                }
                if (count_sales > count_prev_sales)
                {
                    lbl_sales_percent.Content = "+" + string.Format("{0:n}", difference);
                    lbl_sales_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_sales_percent.Content = "" + string.Format("{0:n}", difference);
                    lbl_sales_percent.Foreground = new SolidColorBrush(Colors.Red);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void count_capital(DateTime date)
        {
            double count_sales = 0;
            double count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");


            try
            {
                string query = "select " +
                    " sum(quantity*capital) as count_sales, " +
                    "(select sum(quantity*capital) from sales where date_purchased BETWEEN @date4 AND @date3) as count_prev_sales " +
                    " from " +
                    " sales " +
                    " WHERE " +
                    " date_purchased BETWEEN @date2 AND @date1 ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        count_sales += Double.Parse(reader["count_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales += 0;
                    }

                    try
                    {
                        count_prev_sales += Double.Parse(reader["count_prev_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_prev_sales += 0;
                    }
                }
                connect.Close();

                lbl_capital.Content = string.Format("{0:n}", count_sales);
                difference = count_sales - count_prev_sales;

                // if previous sales = 0 and to avoid dividing by 0
                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales) * 100;
                }
                if (count_sales > count_prev_sales)
                {
                    lbl_capital_percent.Content = "+" + string.Format("{0:n}", difference);
                    lbl_capital_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_capital_percent.Content = "" + string.Format("{0:n}", difference);
                    lbl_capital_percent.Foreground = new SolidColorBrush(Colors.Red);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void count_productSold(DateTime date)
        {
            int count_sales = 0;
            int count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");


            try
            {
                string query = "select " +
                    " sum(quantity) as count_sales, " +
                    "(select sum(quantity) from sales where date_purchased BETWEEN @date4 AND @date3) as count_prev_sales " +
                    " from " +
                    " sales " +
                    " WHERE " +
                    " date_purchased BETWEEN @date2 AND @date1 ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        count_sales = int.Parse(reader["count_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales = 0;
                    }

                    try
                    {
                        count_prev_sales = int.Parse(reader["count_prev_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_prev_sales = 0;
                    }
                }
                connect.Close();

                lbl_sold_products.Content = string.Format("{0:n}", count_sales);
                difference = count_sales - count_prev_sales;

                // if previous sales = 0 and to avoid dividing by 0
                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales) * 100;
                }
                if (count_sales > count_prev_sales)
                {
                    lbl_sold_products_percent.Content = "+" + string.Format("{0:n}", difference);
                    lbl_sold_products_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_sold_products_percent.Content = "" + string.Format("{0:n}", difference);
                    lbl_sold_products_percent.Foreground = new SolidColorBrush(Colors.Red);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void count_gross(DateTime date)
        {
            double count_sales = 0;
            double count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select " +
                    " sum(total) as count_sales," +
                    " (select sum(total) from sales_summary where date_purchased BETWEEN @date4 AND @date3 ) as count_prev_sales " +
                    " FROM " +
                    " sales_summary " +
                    " WHERE " +
                    " date_purchased BETWEEN @date2 AND @date1 ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        count_sales += Double.Parse(reader["count_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales += 0;

                    }

                    try
                    {
                        count_prev_sales += Double.Parse(reader["count_prev_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_prev_sales += 0;
                    }
                }
                connect.Close();
          

                lbl_gross.Content = "\u20B1" + string.Format("{0:n}", count_sales); 
                difference = count_sales - count_prev_sales;

                //if previous sales = 0 and to avoid dividing by 0
                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales);
              
                }

                if (count_sales > count_prev_sales)
                {

                    lbl_gross_percent.Content = "+" + string.Format("{0:n}", difference)  ;
                    lbl_gross_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_gross_percent.Content = "" + string.Format("{0:n}", difference)  ;
                    lbl_gross_percent.Foreground = new SolidColorBrush(Colors.Red);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void count_profit(DateTime date)
        {
            double ave_profit = 0;
            double count_sales = 0;
            double count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select " +
                    " sum(capital*quantity) as capital_now, " +
                    " (select sum(total) from sales_summary where date_purchased Between @date2 AND @date1) as gross_now," +
                    " (select sum(capital*quantity) from sales where date_purchased Between @date4 AND @date3) as capital_prev," +
                    " (select sum(total) from sales_summary where date_purchased Between @date4 AND @date3) as gross_prev," +
                    " sum(quantity) as qty " +
                    " FROM " +
                    " sales " +
                    " where date_purchased BETWEEN @date2 AND @date1";


                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        count_sales = Double.Parse(reader["gross_now"].ToString()) - Double.Parse(reader["capital_now"].ToString())   ;
                        ave_profit = count_sales / Double.Parse(reader["qty"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales = 0;
                    }
                    try
                    {  
                    count_prev_sales = Double.Parse(reader["gross_prev"].ToString()) - Double.Parse(reader["capital_prev"].ToString());
                    } 
                    catch (Exception ex )
                    {
                        count_prev_sales = 0;
                    }

                }
                connect.Close();

                lbl_profit.Content = "\u20B1" + string.Format("{0:n}", count_sales) + " (" + string.Format("{0:n}", ave_profit) + ") "  ;
                difference = count_sales - count_prev_sales;


                // if previous sales = 0 and to avoid dividing by 0

                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales);
                }

                if (count_sales > count_prev_sales)
                {
                    lbl_profit_percent.Content = "+" + string.Format("{0:n}", difference)  ;
                    lbl_profit_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_profit_percent.Content = "" + string.Format("{0:n}", difference) ;
                    lbl_profit_percent.Foreground = new SolidColorBrush(Colors.Red);
                }


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }

        private void count_retailer(DateTime date)
        {
            int count_sales = 0;
            int count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select " +
                    " sum(transaction_type+1) as count_sales, " +
                    "(select sum(transaction_type+1) from  sales_summary where date_purchased BETWEEN @date4 AND @date3 AND transaction_type = @transaction_type ) as count_prev_sales " +
                    "FROM " +
                    "sales_summary " +
                    "WHERE " +
                    "date_purchased BETWEEN @date2 AND @date1 " +
                    "AND transaction_type = @transaction_type ";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Parameters.AddWithValue("@transaction_type", "Retailer");
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {


                    try
                    {
                        count_sales = int.Parse(reader["count_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales += 0;
                    }

                    try
                    {
                        count_prev_sales = int.Parse(reader["count_prev_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_prev_sales += 0;
                    }

                }
                connect.Close();

                lbl_retailers.Content =   string.Format("{0:n}", count_sales);
                difference = count_sales - count_prev_sales;


                // if previous sales = 0 and to avoid dividing by 0

                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales);
                }

                if (count_sales > count_prev_sales)
                {
                    lbl_retailers_percent.Content = "+" + string.Format("{0:n}", difference);
                    lbl_retailers_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_retailers_percent.Content = "" + string.Format("{0:n}", difference);
                    lbl_retailers_percent.Foreground = new SolidColorBrush(Colors.Red);
                }


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }

          private void count_reseller(DateTime date)
        {
            double count_sales = 0;
            double count_prev_sales = 0;
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select " +
                   " sum(transaction_type+1) as count_sales, " +
                   "(select sum(transaction_type+1) from  sales_summary where date_purchased BETWEEN @date4 AND @date3 AND transaction_type = @transaction_type ) as count_prev_sales " +
                   "FROM " +
                   "sales_summary " +
                   "WHERE " +
                   "date_purchased BETWEEN @date2 AND @date1 " +
                   "AND transaction_type = @transaction_type ";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Parameters.AddWithValue("@transaction_type", "Reseller");
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        count_sales = int.Parse(reader["count_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_sales += 0;
                    }
                    try
                    {
                        count_prev_sales += int.Parse(reader["count_prev_sales"].ToString());
                    }
                    catch (Exception ex)
                    {
                        count_prev_sales = 0;
                    }

                }
                connect.Close();

                lbl_resellers.Content = string.Format("{0:n}", count_sales);
                difference = count_sales - count_prev_sales;


                // if previous sales = 0 and to avoid dividing by 0

                if (count_prev_sales == 0)
                {
                    percent = (double)count_sales;
                }
                else
                {
                    percent = ((double)count_sales / (double)count_prev_sales);
                }

                if (count_sales > count_prev_sales)
                {
                    lbl_resellers_percent.Content = "+" + string.Format("{0:n}", difference);
                    lbl_resellers_percent.Foreground = new SolidColorBrush(Colors.SeaGreen);
                }
                else
                {
                    lbl_resellers_percent.Content = "" + string.Format("{0:n}", difference);
                    lbl_resellers_percent.Foreground = new SolidColorBrush(Colors.Red);
                }


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }

        private void count_trendingProduct(DateTime date)
        {
            string count_sales = "";
            string count_prev_sales = "";
            lbl_trending_product_percent.Content = "";
            double difference = 0;
            double percent = 0;

            string date1 = "";
            string date2 = "";
            string date3 = "";
            string date4 = "";

            // Get previous months first and last day to SQL FORMAT ( YEAR / MONTH / DAY )
            var month = new DateTime(date.Year, date.Month, 1);


            //SQL Format get last day of selected month 
            date1 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of selected month 
            date2 = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");

            // SQL Format  get Last day of previous month
            date3 = month.AddDays(-1).ToString("yyyy-MM-dd");

            // SQL Format get 1st day of previous month 
            date4 = month.AddMonths(-1).ToString("yyyy-MM-dd");

            try
            {
                string query = "select sum(s.quantity) as qty_sales," +
                    "p.product_name as product_sales " +
                    "from " +
                    "inventory p " +
                    "inner join " +
                    "sales s " +
                    "where " +
                    "s.product_id = p.product_id " +
                    "AND s.date_purchased BETWEEN @date2 AND @date1 " +
                    " group by s.product_id " +
                    " order by qty_sales desc limit 1";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Parameters.AddWithValue("@date3", date3);
                cmd.Parameters.AddWithValue("@date4", date4);
                cmd.Parameters.AddWithValue("@transaction_type", "Reseller");
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        count_sales =  reader["product_sales"].ToString() ;

                    }
                    catch (Exception ex)
                    {
                        count_sales = "" ;
                    }

                    try
                    {
                        lbl_trending_product_percent.Content  = reader["qty_sales"].ToString()+ " sold items";
                    }
                    catch (Exception ex)
                    {
                        lbl_trending_product_percent.Content  = "";
                    }

                }
                connect.Close();

                lbl_trending_product.Content = count_sales;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }


        private void show_sales(DateTime date)
        {
            String date1, date2 = "";
            
            var month = (date == DateTime.Now) ? new DateTime(DateTime.Now.Year, date.Month, 1) : new DateTime(date.Year, date.Month, 1);

            //SQL Format get first and last day of parameter value
            date1 = month.ToString("yyyy-MM-dd");        
            date2 = month.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            Console.WriteLine(date1 + " " +date2);
            try
            {
                string query = "select " +
                    "s.invoice_num," +
                    "s.customer_name ," +
                    "s.customer_address ," +
                    "s.customer_contact ," +
                    "s.date_purchased ," +
                    "s.total as total, " +
                    "s.transaction_type as customer_type " +
                    "FROM " +
                    "sales s " +
                    "WHERE s.date_purchased BETWEEN @date1 AND @date2 " +
                    "group by s.invoice_num " +
                    "order by invoice_num desc";
                   

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Prepare();
                DataSet ds = new DataSet();
                DataTable dTable = new DataTable();

                MyAdapter.Fill(ds, "LoadDataBinding");
                tbl_reports.DataContext = ds;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void sort_sales(String date1,String date2)
        {
            try
            {
                string query = "select " +
                    "s.invoice_num," +
                    "s.customer_name ," +
                    "s.customer_address ," +
                    "s.customer_contact ," +
                    "s.date_purchased," +
                    "s.total as total, " +
                    "s.transaction_type as customer_type " +
                    "FROM " +
                    "sales s " +
                    "WHERE " +
                    "date_purchased BETWEEN @date2 AND @date1 " +
                    "group by s.invoice_num " +
                    "order by invoice_num desc";


                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@date1", date1);
                cmd.Parameters.AddWithValue("@date2", date2);
                cmd.Prepare();
                DataSet ds = new DataSet();
                DataTable dTable = new DataTable();

                MyAdapter.Fill(ds, "LoadDataBinding");
                tbl_reports.DataContext = ds;
                //tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void btn_view(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            String invoice = dataRowView["invoice_num"].ToString();
            popup.view_purchase view = new popup.view_purchase(this,invoice,1);
            view.ShowDialog();
        }

        private void btn_delete(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            String invoice = dataRowView["invoice_num"].ToString();
            popup.view_purchase view = new popup.view_purchase(this,invoice,0);
            view.ShowDialog();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            popup.month_report sort = new popup.month_report(this);
            sort.ShowDialog();
        }


        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //    if (start_date.Text == "" || end_date.Text == "")
        //    {
        //        MessageBox.Show("No Date Selected", "Reports", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    DateTime start = DateTime.Parse(start_date.Text);
        //    DateTime end = DateTime.Parse(end_date.Text);

        //    String date1 = start.ToString("yyyy-MM-dd");
        //    String date2 = end.ToString("yyyy-MM-dd");


        //    lbl_header.Content = "Data Sorted : " +end.ToShortDateString()+ " - " +start.ToShortDateString();
        //    sort_sales(date1, date2);
        //    count_sales(date1, date2);
        //    count_profit(date1, date2);
        //    count_gross(date1, date2);

        //}

        //private void dteSelectedMonth_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        //{
        //    dteSelectedMonth.DisplayMode = CalendarMode.Year;
        //}

    }
}
