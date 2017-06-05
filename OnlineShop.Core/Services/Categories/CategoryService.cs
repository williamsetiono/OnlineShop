using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models.Categories;
using OnlineShop.Core.Utils;
using OnlineShop.Models.Entities;
using OnlineShop.Models.Repositories;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Categories
{
    public class CategoryService : BaseService<Category> , ICategoryService
    {
        
        public CategoryService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public IQueryable<Category> GetAll()
        {
            return Repository.GetAll();
        }

        public Category FindById(int id)
        {
            return Repository.Get(id);
        }

        public void Insert(Category entityModel)
        {
            if (Repository.GetAll(a => a.Name.Equals(entityModel.Name)).Any())
            {
                throw new Exception(string.Format("Category {0} is exist", entityModel.Name));
            }
            var ancestor = UtilString.GenTempName(entityModel.Name);

            var parent = Repository.Get(entityModel.ParentId);
            if (parent != null)
            {
                entityModel.ParentId = parent.Id;
                ancestor = parent.Ancestor + "|" + ancestor;
            }
            entityModel.Ancestor = ancestor;
            Repository.Insert(entityModel);
            UnitOfWork.Save();
        }

        public void Update(Category entityModel)
        {
            var entity = Repository.Get(entityModel.Id);
            if (entity == null)
                throw new Exception(string.Format("{0} is not found",entityModel.Name));
            if (Repository.GetAll(a => a.Name.Equals(entityModel.Name) && a.Id != entity.Id).Any())
            {
                throw new Exception(string.Format("Category {0} is exist", entity.Name));
            }
            var ancestor = UtilString.GenTempName(entityModel.Name);
            var parent = Repository.Get(entityModel.ParentId);
            if (parent != null)
            {
                entity.ParentId = parent.Id;
                ancestor = parent.Ancestor + "|" + ancestor;
            }
            entity.Name = entityModel.Name;
            var children = Repository.GetAll(a => a.Ancestor.StartsWith(entity.Ancestor));
            foreach (var child in children)
            {
                child.Ancestor = child.Ancestor.Replace(entity.Ancestor, ancestor);
            }
            entity.Ancestor = ancestor;
            Repository.Update(entity);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var entity = Repository.Get(id);
            if (entity == null)
                throw new Exception(string.Format("Category is not found"));
            foreach (var category in entity.Category1)
            {
                var ancestor = category.Ancestor.Split('|');
                category.ParentId = null;
                category.Ancestor = ancestor.Last();
            }
            Repository.Update(entity.Category1);
            Repository.Delete(entity);
            UnitOfWork.Save();
        }
    }
}
