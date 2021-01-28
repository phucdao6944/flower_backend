using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class ResponseListForm<T>
    {
        public string message { get; set; }
        public int code { get; set; }
        public IEnumerable<T> data { get; set; }
        public Pagination pagination { get; set; }
        public ResponseListForm(IEnumerable<T> data, Pagination page, string message = "ok", int code = 200)
        {
            this.message = message;
            this.code = code;
            this.data = data;
            this.pagination = page;
        }
        public ResponseListForm(IEnumerable<T> data, string message = "ok", int code = 200)
        {
            this.message = message;
            this.code = code;
            this.data = data;
        }
    }
}
