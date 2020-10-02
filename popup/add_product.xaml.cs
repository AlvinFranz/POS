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
    /// Interaction logic for add_product.xaml
    /// </summary>
    public partial class add_product : Window
    {
        String image_text = "";
        private Forms.Inventory inventory;
        public add_product(Forms.Inventory inventory1)
        {
            InitializeComponent();
            inventory = inventory1;
            populate_supplier();
            populate_category();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clear_details();


        }

        private void clear_details()
        {
            prod_image.Source = null;
            image_text = "";
            cbox_supplier.Text = "";
            cbox_category.Text = "";
            txt_capital.Clear();
            txt_code.Clear();
            txt_description.Clear();
            txt_price.Clear();
            txt_quantity.Clear();
        }

        private void insert_product()
        {
            String dateNow = DateTime.Now.ToString("yyyy-MM-dd");
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
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save product details?", "Add Product", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "insert into inventory values " +
                        "(@product_id," +
                        "@product_name," +
                        "(select category_id from category where category_name = @category_name)," +
                        "(select supplier_id from supplier where supplier_name = @supplier_name)," +
                        "@product_capital," +               
                        "@product_price," +
                        "@product_quantity," +
                        "@product_image," +
                        "@dateNow) ";
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
                    cmd.Parameters.AddWithValue("@dateNow", dateNow);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Saved Data!", "Add Product", MessageBoxButton.OK, MessageBoxImage.Information);

                    clear_details();

                    inventory.show_inventory();

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

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
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            insert_product();
        }
    }
}
