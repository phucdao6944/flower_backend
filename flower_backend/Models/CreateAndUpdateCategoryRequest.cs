using flower.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class CreateAndUpdateCategoryRequest
    {
        [Required]
        [MinLength(1)]
        public string name { set; get; }
    }
}
