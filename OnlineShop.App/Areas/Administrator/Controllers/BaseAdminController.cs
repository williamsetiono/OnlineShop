using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineShop.App.Securities;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class BaseAdminController : Controller
    {
        //TODO: some common function
        protected readonly IUnitOfWork UnitOfWork ;
        public BaseAdminController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            ViewBag.BaseSetting = GetBaseSetting();

        }
        protected Dictionary<int, string> GetCategoryDictionary(int? categoryid = null)
        {
            
             var dictionary = categoryid.HasValue 
                 ? UnitOfWork.Repository<Category>().GetAll(a=>a.Id != categoryid && a.ParentId == null).ToDictionary(a => a.Id, a => a.Name) 
                 : UnitOfWork.Repository<Category>().GetAll(a=> a.ParentId == null).ToDictionary(a => a.Id, a => a.Name);

            dictionary.Add(-1, "None");
             return dictionary;
        }
        protected Dictionary<int, string> GetCategoryDictionary()
        {

            var dictionary = UnitOfWork.Repository<Category>().GetAll().ToDictionary(a => a.Id, a => a.Name);
            dictionary.Add(-1, "None");
             return dictionary;
        }

        protected Account CurrentUser()
        {
            return UnitOfWork.Repository<Account>().GetAll(a=>a.Username.Equals(User.Identity.Name)).FirstOrDefault();
        }

        protected Dictionary<int, string> GetRoleDictionary()
        {
            return UnitOfWork.Repository<Role>().GetAll().ToDictionary(x => x.Id, x => x.Name);
        }
        protected Dictionary<string, string> GetBaseSetting()
        {
            return UnitOfWork.Repository<Setting>().GetAll()
                .Where(a => a.Type.Equals("BaseSetting"))
                .ToDictionary(a => a.Id, a => a.Value);
        }
        protected string GetCurrencySign()
        {
            return "$";
        }
	}
}