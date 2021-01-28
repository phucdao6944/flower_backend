using flower.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class Order
    {
        public int id { get; set; }
        public User user { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public StatusEnum status { get; set; }
        public DateTime expected_time { get; set; }
        public DateTime created_at { get; set; }
        public double total_price { get; set; }
        public bool paid { get; set; }
        public List<OrderDetail> orderDetails { set; get; }
    }
}
