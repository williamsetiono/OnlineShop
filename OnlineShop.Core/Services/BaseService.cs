using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Models.Repositories;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services
{
    public class BaseService<T> : IBaseService<T> where T: class
    {
        protected readonly IUnitOfWork UnitOfWork;
        public BaseService( IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Repository = unitOfWork.Repository<T>();
        }

        public void Dispose()
        {
           UnitOfWork.Dispose();
        }

        public IRepositoryBase<T> Repository { get; private set; }
    }
}
