using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class Bouquet
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public double price { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<BouquetImage> images { get; set; }
    }
}
