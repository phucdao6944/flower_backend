using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime? dob { get; set; }
        public bool? gender { get; set; }
        public string phone_number { get; set; }
        [MinLength(10)]
        public string address { get; set; }
    }
}
