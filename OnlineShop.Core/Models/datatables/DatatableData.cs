using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.datatables
{
    public class DatatableData<T> where T: class
    {
        public ICollection<T> Data { get; set; }
        public int Index { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public int StartIndex { get; set; }

    }
}
