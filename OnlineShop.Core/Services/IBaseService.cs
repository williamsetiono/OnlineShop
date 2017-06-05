using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models;
using OnlineShop.Models.Repositories;

namespace OnlineShop.Core.Services
{
    public interface IBaseService<T> : IDisposable where T: class
    {
        IRepositoryBase<T> Repository { get; }
    }
}
