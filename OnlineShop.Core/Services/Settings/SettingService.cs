using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models.Settings;
using OnlineShop.Core.Provider;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Settings
{
    public class SettingService : BaseService<Setting>,ISettingService
    {
        public SettingService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Setting> GetAll()
        {
            return Repository.GetAll();
        }

        public Setting FindById(string id)
        {
            return Repository.Get(id);
        }

        public void Insert(SettingVIewModel model)
        {

            var entity = new Setting()
            {
                Id = model.Id,
                Type = model.Type,
                Value = model.Value
            };
            Repository.Insert(entity);
            UnitOfWork.Save();
        }

        public void Update(SettingVIewModel model)
        {
            if (string.IsNullOrEmpty(model.Id)) throw new Exception("Setting key is null");
            var entity = Repository.Get(model.Id);
            if (entity == null) throw new Exception("Setting key is not found");
            //entity.Id = model.Id;
            entity.Type = model.Type;
            entity.Value = model.Value;
            Repository.Update(entity);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var entity = Repository.Get(id);
            if (entity == null) throw new Exception("Setting key is not found");
            Repository.Delete(entity);
            UnitOfWork.Save();
        }
        private string GetSystemType(int key)
        {
            return Constants.TypeDictionary.Any(a => a.Key == key)
                      ? Constants.TypeDictionary[key]
                      : Constants.TypeDictionary[(int)Constants.EnumType.String];
        }
    }
}
