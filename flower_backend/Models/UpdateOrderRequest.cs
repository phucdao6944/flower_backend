using flower.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class UpdateOrderRequest
    {     
        public string address { set; get; }
        public string phone_number { set; get; }
        public StatusEnum status { set; get; }
    }
}
