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
using System.Windows.Threading;

namespace POS.Forms
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl
    {
        //public static DataGrid 
        System.Windows.Threading.DispatcherTimer _typingTimer;
        public Inventory()
        {
            InitializeComponent();
            show_inventory();
        }
        public void show_inventory()
        {
            try
            {
                string query = "select " +
                    "i.product_id as id," +
                    "i.product_name as name," +
                    "c.category_name as category," +
                    "s.supplier_name as supplier," +
                    "i.product_capital as capital," +
                    "i.product_price as price," +
                    "i.product_quantity as quantity," +
                    "i.date_registered as date_registered," +
                    "i.product_image as image " +
                    "FROM " +
                    "inventory i, " +
                    "category c " +
                    "INNER JOIN " +
                    "supplier s " +
                    "WHERE " +
                    "i.category_id = c.category_id AND " +
                    "i.supplier_id = s.supplier_id " +
                    "order by c.category_name";
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
                tbl_inventory.DataContext = ds;
                //tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            open_addProduct();  
        }

        public void open_addProduct()
        {
            popup.add_product add_product = new popup.add_product(this);
            add_product.ShowDialog();
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    open_addProduct();
                    break;

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            popup.add_category category = new popup.add_category();
            category.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            popup.supplier supplier = new popup.supplier();
            supplier.ShowDialog();
        }
        private void open_product_detail(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            string prod_id = dataRowView["id"].ToString();
            popup.product_detail detail = new popup.product_detail(prod_id);
            detail.ShowDialog();
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

            string query = "select " +
            "i.product_id as id," +
            "i.product_name as name," +
            "c.category_name as category," +
            "s.supplier_name as supplier," +
            "i.product_capital as capital," +
            "i.product_price as price," +
            "i.product_quantity as quantity," +
            "i.date_registered as date_registered," +
            "i.product_image as image " +
            "FROM " +
            "inventory i, " +
            "category c " +
            "INNER JOIN " +
            "supplier s " +
            "WHERE " +
            "i.category_id = c.category_id AND " +
            "i.supplier_id = s.supplier_id AND " +
            "(i.product_id LIKE @search OR " +
            "i.product_name LIKE @search OR " +
            "c.category_name LIKE @search OR  " +
            "s.supplier_name LIKE @search OR " +
            "i.product_capital LIKE @search OR " +
            "i.product_price LIKE @search OR " +
            "i.product_quantity LIKE @search) " +
            "order by c.category_name";

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
            tbl_inventory.ItemsSource = dTable.DefaultView;

            connect.Close();
            timer.Stop();
        }
        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_typingTimer == null)
            {
           
                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(400);

                _typingTimer.Tick += new EventHandler(this.handleTypingTimerTimeout);
            }
            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Tag = (sender as TextBox).Text; // This should be done with EventArgs
            _typingTimer.Start();

        }
        private void delete_product(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            String  prod_id = dataRowView["id"].ToString();
            try {  
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you wish to delete Product details? \n Product ID: " + @prod_id , "Product", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "delete from inventory where product_id= @prod_id";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@prod_id", prod_id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Removed Data!", "Product", MessageBoxButton.OK, MessageBoxImage.Information);
                    show_inventory();
                    connect.Close();
                }
                else
                {
                     return;

                }
            } 
            catch (Exception ex )
            {
                MessageBox.Show("Unable to Removed Data!", "Product", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


        }
        private void update_product(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            String prod_id = dataRowView["id"].ToString();
            popup.edit_product edit_product = new popup.edit_product(this,prod_id);
            edit_product.ShowDialog();
        }

        private void tbl_inventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
