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
    /// Interaction logic for add_category.xaml
    /// </summary>
    public partial class add_category : Window
    {
        public add_category()
        {
            InitializeComponent();
            showCategory();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txt_categoryName.Text == "" )
            {
                MessageBox.Show("Incomplete details");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save category details?", "Add Category", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into category values ('',@category_name) ";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@category_name", txt_categoryName.Text);
                 
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Saved Data!", "Add Category", MessageBoxButton.OK, MessageBoxImage.Information);
                    txt_categoryName.Clear();
                    showCategory();
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
        public void showCategory()
        {
            string query = "select * from category";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            tbl_category.ItemsSource = dTable.DefaultView;
            connect.Close();
        }
        private void delete_category(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
            int category_id = int.Parse(dataRowView["category_id"].ToString());

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you wish to delete Category details? \n Category ID: @category_id", "Category", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string query = "delete from category where category_id= @category_id";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@category_id", category_id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully Removed Data!", "Category", MessageBoxButton.OK, MessageBoxImage.Information);
                showCategory();
                connect.Close();
            }
            else
            {
                MessageBox.Show("Unable to Remove Data!", "Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }
    }
}
