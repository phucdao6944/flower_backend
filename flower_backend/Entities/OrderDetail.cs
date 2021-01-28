using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class OrderDetail
    {
        public int order_id { set; get; }
        public int bouquet_id { set; get; }
        public string name { set; get; }
        public double price { set; get; }
        public int quantity { set; get; }
        public string message { set; get; }
        public List<BouquetImage> images { get; set; }
    }
}
