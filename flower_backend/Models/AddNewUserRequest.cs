using flower.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class AddNewUserRequest
    {
        [Required]
        public string username { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [Required]
        public string first_name { get; set; }
        public string? last_name { get; set; }
        public DateTime? dob { get; set; }
        public bool? gender { get; set; }
        [Required]
        public string phone_number { get; set; }
        [Required]
        [MinLength(10)]
        public string address { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Invalid password. Use 8 or more characters with a mix of letters, numbers & symbols")]
        public string password { get; set; }
    }
}
