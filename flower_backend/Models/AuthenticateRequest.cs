using System.ComponentModel.DataAnnotations;

namespace flower.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        public bool admin { get; set; }
    }
}