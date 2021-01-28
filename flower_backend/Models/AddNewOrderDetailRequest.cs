using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class AddNewOrderDetailRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int bouquet_id { set; get; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Required]
        public int quantity { set; get; }
        [Required]
        public string message { set; get; }
    }
}
