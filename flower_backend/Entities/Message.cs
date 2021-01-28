using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class Message
    {
        public int id { get; set; }
        public string messages { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
