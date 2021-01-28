using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class ResponseForm<T>
    {
        public string message { get; set; }
        public int code { get; set; }
        public T data { get; set; }
        public ResponseForm(T data, string message = "ok", int code = 200)
        {
            this.message = message;
            this.code = code;
            this.data = data;
        }
    }
}
