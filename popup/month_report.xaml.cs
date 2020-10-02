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
    /// Interaction logic for month_report.xaml
    /// </summary>
    public partial class month_report : Window
    {
        private Forms.Reports report;
   
        public month_report(Forms.Reports report1)
        {
           
            InitializeComponent();    
            report = report1;
        }
  

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void dteSelectedMonth_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            dteSelectedMonth.DisplayMode = CalendarMode.Year;
            
        }
        private void dteSelectedMonth_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        { 
             report.sort_date(dteSelectedMonth.DisplayDate.ToString("MMMM-yyyy"));   
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
