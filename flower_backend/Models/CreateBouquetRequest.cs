using flower.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class CreateBouquetRequest
    {
        [Required]
        [MinLength(5)]
        public string name { get; set; }

        [Required]
        [MinLength(20)]
        public string description { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int category_id { get; set; }
        [Required]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public float price { get; set; }
        [Required]
        [Base64ImageArrayStrings]
        public string[] images { get; set; }
    }
}
