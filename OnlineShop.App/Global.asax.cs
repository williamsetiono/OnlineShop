using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using Autofac.Integration.Mvc;
using datatables.Utils.DatatableModels;
using datatables.Utils.ModelBinder;
using Newtonsoft.Json;
using OnlineShop.App.Areas.Administrator.Controllers;
using OnlineShop.App.Securities;
using OnlineShop.Core.Services;
using OnlineShop.Core.Services.Categories;
using OnlineShop.Core.Services.Coupons;
using OnlineShop.Core.Services.Orders;
using OnlineShop.Core.Services.Permissions;
using OnlineShop.Core.Services.Products;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            //builder.RegisterType<CustomMemberShipProvider>();
            //builder.Register(c => c.Resolve<CustomMemberShipProvider>()).As<MembershipProvider>();
            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            #region components

            var assembly = Assembly.GetAssembly(typeof (BaseService<>));

            builder.RegisterType<EntityContext>().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<UserContext>().As<IUserContext>().InstancePerDependency();
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<OrdersService>().As<IOrdersService>();
            builder.RegisterType<SettingService>().As<ISettingService>();
            builder.RegisterType<PermissionService>().As<IPermissionService>();
            builder.RegisterType<CouponService>().As<ICouponService>();
            builder.RegisterType<UserService>().As<IUserService>();

 
            #endregion

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Custom binding
            ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(string), new StringModelBinder());
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(cookie.Value);
                var ma = JsonConvert.DeserializeObject<MyAccount>(authTicket.UserData);
                var mp = new MyPrincipal(ma.Username) {Ma = ma};
                HttpContext.Current.User = mp;
            }
        }
    }
}
