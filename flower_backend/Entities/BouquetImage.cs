using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class BouquetImage
    {
        public int id { set; get; }
        public string image_url { set; get; }
        public int bouquet_id { set; get; }
    }
}
