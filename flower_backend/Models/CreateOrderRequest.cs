using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class CreateOrderRequest
    {
        [Required]
        public string address { set; get; }
        [Required]
        public string phone_number { set; get; }
        [Required]
        public List<AddNewOrderDetailRequest> order_details { set; get; }
    }
}
