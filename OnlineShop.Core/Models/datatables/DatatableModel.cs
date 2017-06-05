using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.datatables
{
    public static class DatatableModel
    {

        public static DatatableData<T> Refresh<T>(IQueryable<T> model, int size = 25, int index = 1) where T : class
        {
            var data = new DatatableData<T>();
            data.Total = model.Count();
            if (size == 0)
            {
                size = 25;
            }
            data.Size = size;
            if (index < 1)
            {
                index = 1;
            }
            data.Index = index;
            var page = data.Total % data.Size;
            data.Page = page == 0 ? (data.Total / data.Size) : (data.Total / data.Size) + 1;
            if (data.Index > data.Page)
            {
                data.Index = 1;
            }
            data.StartIndex = (data.Index - 1) * size;
            data.Data = model.Skip(data.StartIndex).Take(size).ToList();
            return data;
        }
       
    }
}
