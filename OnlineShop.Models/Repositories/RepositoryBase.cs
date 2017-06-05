using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using OnlineShop.Models.Entities;

namespace OnlineShop.Models.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T: class
    {
        private readonly EntityContext _context;
        private const string EntityNotFound = "Data object is not found";
        public RepositoryBase(EntityContext context)
        {
            _context = context;
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void Insert(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Insert(ICollection<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null)
        {
            return expression == null ? _context.Set<T>() : _context.Set<T>().Where(expression);

        }

        public T Get(object key)
        {
            return _context.Set<T>().Find(key);
        }
    }
}
