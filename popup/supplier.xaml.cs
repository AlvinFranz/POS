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
using System.Windows.Shapes;

namespace POS.popup
{
    /// <summary>
    /// Interaction logic for supplier.xaml
    /// </summary>
    public partial class supplier : Window
    {
        public supplier()
        {
            InitializeComponent();
            show_supplier();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void show_supplier()
        {
            string query = "select * from supplier";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            tbl_supplier.ItemsSource = dTable.DefaultView;
            connect.Close();
        }

        private void search_supplier()
        {
            string query = "select * from supplier " +
                            "WHERE " +
                            "supplier_name LIKE @search " +
                            "OR supplier_contact LIKE @search " +
                            "OR supplier_address LIKE @search ";

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
            tbl_supplier.ItemsSource = dTable.DefaultView;

            connect.Close();
        }


        private void btn_save_click(object sender, RoutedEventArgs e)
        {
            if (txt_supplierAddress.Text == "" || txt_supplierContact.Text == "" || txt_supplierName.Text == "")
            {
                MessageBox.Show("Incomplete details");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save Supplier details?", "Add Supplier", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into supplier values ('',@supplier_name,@supplier_contact,@supplier_address) ";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@supplier_name", txt_supplierName.Text);
                    cmd.Parameters.AddWithValue("@supplier_address", txt_supplierAddress.Text);
                    cmd.Parameters.AddWithValue("@supplier_contact", txt_supplierContact.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Saved Data!", "Add Supplier", MessageBoxButton.OK, MessageBoxImage.Information);


                    clear_details();
                    show_supplier();
                    Forms.Inventory inventory = new Forms.Inventory();
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
        private void clear_details()
        {
            txt_supplierName.Clear();
            txt_supplierContact.Clear();
            txt_supplierAddress.Clear();
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            clear_details();
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_supplier();
        }
        private void delete_supplier(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            String supplier_id = dataRowView["supplier_id"].ToString();

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you wish to delete Supplier details? \n Supplier ID: @supplier_id", "Supplier", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string query = "delete from supplier where supplier_id = @supplier_id";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@supplier_id", supplier_id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully Removed Data!", "Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
                show_supplier();    
                connect.Close();
            }
            else
            {
                MessageBox.Show("Unable to Remove Data!", "Supplier", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }
    }
}
