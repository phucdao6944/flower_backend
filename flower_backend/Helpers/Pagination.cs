using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class Pagination
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public int? total_records { get; set; }
        private int? total_pages => total_records.HasValue ? (int)Math.Ceiling(total_records.Value / (double)limit) : (int?)null;

        public Pagination(int limit, int offset, int total)
        {
            this.limit = limit;
            this.offset = offset;
            this.total_records = total;
        }
        public Pagination()
        {
        }
    }
}
