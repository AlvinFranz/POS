using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for edit_product.xaml
    /// </summary>
    public partial class edit_product : Window
    {
        String image_text = "";
        String product_id = "";
        private Forms.Inventory inventory;
        public edit_product(Forms.Inventory inventory1,String prod_id)
        {
            InitializeComponent();
            product_id = prod_id;
            inventory = inventory1;
            populate_supplier();
            populate_category();
            product_detail();
        }

        private void product_detail()
        {
            try
            {
                string query = "select " +
                     "i.product_id as id," + //
                     "i.product_name as name," + //
                     "c.category_name as category," +
                     "s.supplier_name as supplier," +
                     "i.product_capital as capital," + //
                     "i.product_price as price," + //
                     "i.product_quantity as quantity," + //
                     "i.product_image as image " +
                     "FROM " +
                     "inventory i, " +
                     "category c " +
                     "INNER JOIN " +
                     "supplier s " +
                     "WHERE " +
                     "i.product_id = @product_id AND " +
                     "i.category_id = c.category_id AND " +
                     "i.supplier_id = s.supplier_id ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@product_id", product_id);

                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    txt_capital.Text = reader["capital"].ToString();
                    txt_quantity.Text = reader["quantity"].ToString();
                    txt_price.Text = reader["price"].ToString();
                    txt_description.Text = reader["name"].ToString();
                    txt_code.Text = reader["id"].ToString();
                    cbox_category.SelectedItem = reader["category"];
                    cbox_supplier.SelectedItem = reader["supplier"];

                    byte[] data = (byte[])reader["image"];

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
                return;
            }
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
        private void open_image_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG;)|*.png;*.jpeg;*.JPG;";
            if (openFileDialog.ShowDialog() == true)
            {
                String file = (openFileDialog.FileName).ToString();
                image_text = file;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                prod_image.Source = bitmap;

            }
        }
        private void populate_supplier()
        {
            try
            {
                string query = "select supplier_name from supplier";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbox_supplier.Items.Add((string)reader["supplier_name"]);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void populate_category()
        {
            try
            {
                string query = "select category_name from category";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbox_category.Items.Add((string)reader["category_name"]);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }


        private void btn_update_click(object sender, RoutedEventArgs e)
        {
            String query = "";
            if (image_text == "")
            {
                 query = "update inventory set " +                    
                         "product_name = @product_name, " +
                         "category_id = (select category_id from category where category_name = @category_name), " +
                         "supplier_id = (select supplier_id from supplier where supplier_name = @supplier_name), " +
                         "product_capital = @product_capital, " +
                         "product_price = @product_price, " +
                         "product_quantity = @product_quantity "  +
                         " where product_id = @product_id";
            }
            else
            {
                query = "update inventory set " +
                         "product_name = @product_name, " +
                         "category_id = (select category_id from category where category_name = @category_name), " +
                         "supplier_id = (select supplier_id from supplier where supplier_name = @supplier_name), " +
                         "product_capital = @product_capital, " +
                         "product_price = @product_price, " +
                         "product_quantity = @product_quantity," +
                         "product_image = @product_image " +
                         " where product_id = @product_id";
            }
            FileStream fs;
            BinaryReader br;
            byte[] ImageData = new byte[0];
            string FileName = "";

            if (image_text != "")
            {
                FileName = image_text;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            if (cbox_supplier.Text == "" || cbox_category.Text == "")
            {
                MessageBox.Show("Incomplete details");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Update product details?", "Edit Product", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@product_id", txt_code.Text);
                    cmd.Parameters.AddWithValue("@product_name", txt_description.Text);
                    cmd.Parameters.AddWithValue("@category_name", cbox_category.Text);
                    cmd.Parameters.AddWithValue("@supplier_name", cbox_supplier.Text);
                    cmd.Parameters.AddWithValue("@product_capital", txt_capital.Text);
                    cmd.Parameters.AddWithValue("@product_price", txt_price.Text);
                    cmd.Parameters.AddWithValue("@product_quantity", txt_quantity.Text);
                    cmd.Parameters.AddWithValue("@product_image", ImageData);
        
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Updated Data!", "Edit Product", MessageBoxButton.OK, MessageBoxImage.Information);
                    inventory.show_inventory();

                    connect.Close();
                    this.Close();
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
    }
}
