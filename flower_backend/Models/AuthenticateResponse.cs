using flower.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Models
{
    public class AuthenticateResponse
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            id = user.id;
            first_name = user.first_name;
            last_name = user.last_name;
            username = user.username;
            this.token = token;
        }
    }
}
