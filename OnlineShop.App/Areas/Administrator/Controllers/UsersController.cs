using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Users;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Users;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class UsersController : BaseAdminController
    {
        private readonly IUserService _userService;
        private IUserContext usercontext;
        public UsersController(IUnitOfWork unitOfWork, IUserService userService)
            : base(unitOfWork)
        {
            _userService = userService;
        }
        //
        // GET: /Administrator/Users/
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult GetAll(DataTablesParam request)
        {
            var user = _userService.GetAll().Select(a=>new UserViewModel()
            {
                Id = a.Id,
                UserName = a.Username,
                Status = a.Status,
                Address = a.Address,
                 Email = a.Email,
                 FullName = a.FullName,
                 Phone = a.Phone,
                 Role = a.Role.Name 
            });
            return DataTablesResult.Create(user, request);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new UserViewModel()
            {
                Roles = GetRoleDictionary()
            });
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    _userService.Insert(model);
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = true,
                        Message = string.Format("Create {0} success", model.UserName)
                    };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format("Create {0} failed, error: {1}", model.UserName, ex.Message)
                    };
                }

            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel()
                {
                    Result = false,
                    Message = string.Format("Create failed, error: {0}","Invali data")
                };
            }
            model.Roles = GetRoleDictionary();
            return View(model);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }
            var entity = _userService.FindById(id.Value);
            var model = new UserViewModel()
            {
                Id = entity.Id,
                UserName = entity.Username,
                Status = entity.Status,
                Address = entity.Address,
                 Email = entity.Email,
                 FullName = entity.FullName,
                 Phone = entity.Phone,
                 RoleId = entity.RoleId,
                 Roles = GetRoleDictionary()
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userService.Update(model);
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = true,
                        Message = string.Format("Update {0} success", model.UserName)
                    };
                    return RedirectToAction("index");
                }
                catch (Exception ex)
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format("Update {0} failed, error: {1}", model.UserName, ex.Message)
                    };

                }

            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel()
                {
                    Result = false,
                    Message = string.Format("Update failed, error: {0}","invalid data")
                };
            }
            model.Roles = GetRoleDictionary();
            return View(model);
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                _userService.Delete(id.Value);
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
        public ActionResult Profile()
        {
            var user = CurrentUser();
            var model = new UserInfoViewModel()
            {
                Phone = user.Phone,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                UserName = user.Username
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Profile(UserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userService.Update(model);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                }
            }
            return View(model);
        }
	}
    
}