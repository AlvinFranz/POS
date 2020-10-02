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
    /// Interaction logic for product_detail.xaml
    /// </summary>
    public partial class product_detail : Window
    {
        private String get_code = "";
        public product_detail(String code)
        {
            InitializeComponent();
            get_code = code;
            fill_details(code);
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void fill_details(string code)
        {
            lbl_code.Content = code;

            try
            {
                string query = "select " +
                    "p.product_quantity," +
                    "p.product_name as name," +
                    "p.product_price as price," +
                    "p.date_registered," +
                    "count(s.product_id) as items_sold," +
                    "sum(p.product_capital * p.product_quantity  + (s.capital*s.quantity)) as total_capital," +
                    "(select count(product_id) from sales where product_id = @product_id group by invoice_num desc limit 1) as most_sales,  " +
                    "(select date_purchased from sales where product_id = @product_id order by date_purchased desc limit 1) as recent_purchase," +
                    "sum(s.price*s.quantity) as total_gross," +
                    "sum(s.price*s.quantity)-s.capital as total_revenue, " +
                    "n.note, " +
                    "p.product_image " +
                    "FROM " +
                    "inventory p," +
                    "notes n " +
                    "INNER JOIN " +
                    "sales s " +
                    "WHERE " +
                    "p.product_id = @product_id AND " +
                    "p.product_id = n.product_id AND " +
                    "p.product_id = s.product_id "; 
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@product_id", code);
                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_stocks.Text = reader["product_quantity"].ToString();
                    lbl_description.Content = reader["name"];
                    txt_price.Text = string.Format("{0:n}", reader["price"]); 
                    txt_registered.Text = reader["date_registered"].ToString();
                    txt_qty_sold.Text = reader["items_sold"].ToString();
                    txt_capital.Text = string.Format("{0:n}", reader["total_capital"]);  
                    txt_most_sales.Text = reader["most_sales"].ToString();
                    txt_gross.Text = string.Format("{0:n}", reader["total_gross"]);
                    txt_revenue.Text = string.Format("{0:n}", reader["total_revenue"]);
                    txt_recent_purchase.Text = string.Format("{0:n}", reader["recent_purchase"]);
                    txt_notes.Text = reader["note"].ToString();

                    byte[] data = (byte[])reader["product_image"]; 

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                    {
                        var imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                        prod_image.Source = imageSource;
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

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save product details?", "Product Detail", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string query = "insert into notes values (@product_id,@notes)";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@product_id", get_code);
                cmd.Parameters.AddWithValue("@notes", txt_notes.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully Saved Data!", "Product Detail", MessageBoxButton.OK, MessageBoxImage.Information);

                connect.Close();
                this.Close();
            }
            else
            {
                return;
            }
        }
    }
}
