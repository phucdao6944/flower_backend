using flower.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class UpdateBouquetRequest
    {
        public string name { get; set; }
        [MinLength(20)]
        public string description { get; set; }
        public int category_id { get; set; }
        public float price { get; set; }
        [Base64ImageArrayStrings]
        public string[] images { get; set; }
    }
}
