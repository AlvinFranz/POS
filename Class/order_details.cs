using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Class
{
    class order_details
    {
        public string product_id { get; set; }
        public string product_name { get; set; }
        public int qty { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public double capital { get; set; }

    }
}
