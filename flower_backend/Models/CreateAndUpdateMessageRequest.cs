using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class CreateAndUpdateMessageRequest
    {
        [Required]
        [MinLength(20)]
        public string message { set; get; }
    }
}
