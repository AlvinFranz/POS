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
    /// Interaction logic for add_order.xaml
    /// </summary>
    public partial class add_order : Window
    {
        public Forms.Transaction transaction;

        double capital = 0;
        public add_order(Forms.Transaction transaction1)
        {
         
            InitializeComponent();
            transaction = transaction1;
            txt_qty.CaretIndex = txt_qty.Text.Length;

         
            show_details();
           
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void show_details()
        {
            try
            {
                string query = "select " +
                    "product_id, " +
                    "product_name," +
                    "category_name," +
                    "product_price," +
                    "product_quantity, " +
                    "product_capital " +
                    "FROM " +
                    "inventory i " +
                    "INNER JOIN " +
                    "category c " +
                    "where product_id = @product_id " +
                    "AND " +
                    "i.category_id = c.category_id";
                   
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
            

                cmd.Parameters.AddWithValue("@product_id", transaction.prod_id.ToString());
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    capital = double.Parse(reader["product_capital"].ToString());
                    txt_prodID.Text = ((int)reader["product_id"]).ToString();
                    txt_desc.Text =((string)reader["product_name"]);
                    txt_category.Text = ((string)reader["category_name"]);
                    txt_stocks.Text = ((int)reader["product_quantity"]).ToString();
                    txt_price.Text = string.Format("{0:n}", ((double)reader["product_price"]));
                }
                connect.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void qty_TextChanged(object sender, TextChangedEventArgs e)
        {
            //try { int.Parse(txt_qty.Text); } catch (Exception ex) { txt_qty.Text = "1"; return; }
            try
            {
                if (int.Parse(txt_qty.Text) <= 0)
                {
                    txt_qty.Text = "1";
                }
             
            }
            catch (Exception)
            {
                txt_qty.Text = "1"; return; 
            }

            try
            {
                if (int.Parse(txt_qty.Text) > int.Parse(txt_stocks.Text))
                {
                    txt_qty.Text = txt_stocks.Text;
                    return;
                }

            }
            catch (Exception)
            {
                txt_qty.Text = "1"; return;
            }


        }

        private void inc_qty_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(txt_qty.Text) == int.Parse(txt_stocks.Text))
            {
                txt_qty.Text = txt_stocks.Text;
                return;
            }
            txt_qty.Text = (int.Parse(txt_qty.Text)+1).ToString();
        }

        private void dec_qty_Click(object sender, RoutedEventArgs e)
        {
            if(int.Parse(txt_qty.Text) == 1) { 
                return; 
            } 

            txt_qty.Text = (int.Parse(txt_qty.Text) - 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            foreach (Class.order_details p in transaction.tbl_orders.Items)
            {              
               x++;
                if (p.product_id.Equals(txt_prodID.Text)) {

                    transaction.tbl_orders.Items.RemoveAt(x - 1);
                    break;
                }
            }

            insert_order();
            this.Close();
        }
        private void insert_order ()
        {     
            //Add Products to Cart
            transaction.tbl_orders.Items.Add(new Class.order_details
            {
             
                product_name = txt_desc.Text,
                product_id = txt_prodID.Text,
                capital = capital,
                qty = int.Parse(txt_qty.Text),
                total = int.Parse(txt_qty.Text) * double.Parse(txt_price.Text),
                price = double.Parse(txt_price.Text)
            });;

            // Calculate total amount of selected products
            double total_price = 0;
            foreach (Class.order_details p in transaction.tbl_orders.Items)
            {
                total_price += p.qty * p.price;
            }

            transaction.txt_total.Content = string.Format("{0:n}", total_price);
        }    
    }
}
