using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace flower.Entities
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime dob { get; set; }
        public bool gender { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }
        public bool is_admin { get; set; }
        public string profile_picture { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        [JsonIgnore]
        public string password { get; set; }
    }
}
