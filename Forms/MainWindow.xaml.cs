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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace POS

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            open_dashboard();
        }

        private void open_dashboard()
        {
            Forms.Dashboard dashboard = new Forms.Dashboard();
            stack.Children.Clear();
            stack.Children.Add(dashboard);
        }
        private void open_inventory()
        {
            Forms.Inventory inventory = new Forms.Inventory();
            stack.Children.Clear();
            stack.Children.Add(inventory);
        }

        private void open_transaction()
        {
            Forms.Transaction Transaction = new Forms.Transaction();
            stack.Children.Clear();
            stack.Children.Add(Transaction);
        }


        private void Dashboard_Selected(object sender, RoutedEventArgs e)
        {
            open_dashboard();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void Customers_Selected(object sender, RoutedEventArgs e)
        {
            Forms.Customers customer = new Forms.Customers();
            customer.ShowDialog();
        }

        private void Transaction_Selected(object sender, RoutedEventArgs e)
        {
            open_transaction();
        }

        private void Inventory_Selected(object sender, RoutedEventArgs e)
        {
            open_inventory();
        }

        private void Reports_Selected(object sender, RoutedEventArgs e)
        {
            Forms.Reports reports = new Forms.Reports();
            stack.Children.Clear();
            stack.Children.Add(reports);
        }
    }
}
