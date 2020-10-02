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
    /// Interaction logic for edit_customer.xaml
    /// </summary>
    public partial class edit_customer : Window
    {
        private Forms.Customers customer;
        public edit_customer(Forms.Customers customer1)
        {
            InitializeComponent();
            customer = customer1;
            customer_detail();
            cursor_last();
        }
       
        private void cursor_last()
        {
            txt_customerAddress.CaretIndex = txt_customerAddress.Text.Length;
            txt_customerContact.CaretIndex = txt_customerContact.Text.Length;
            txt_customerName.CaretIndex = txt_customerName.Text.Length;
        }
        public void customer_detail ()
        {
            txt_customerAddress.Text = customer.customer_address;
            txt_customerContact.Text = customer.customer_contact;
            txt_customerID.Text = customer.customer_id.ToString();
            txt_customerName.Text = customer.customer_name;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void save(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Update Customer details?", "Edit Customer", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "update customer SET " +
                            " customer_name = @customer_name," +
                            " customer_address = @customer_address," +
                            " customer_contact = @customer_contact " +
                            " WHERE " +
                            " customer_id = @customer_id";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@customer_id", txt_customerID.Text);
                    cmd.Parameters.AddWithValue("@customer_contact", txt_customerContact.Text);
                    cmd.Parameters.AddWithValue("@customer_address", txt_customerAddress.Text);
                    cmd.Parameters.AddWithValue("@customer_name", txt_customerName.Text);             
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Updated Data!", "Edit Customer", MessageBoxButton.OK, MessageBoxImage.Information);
                    customer.show_customers();

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

        private void cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
