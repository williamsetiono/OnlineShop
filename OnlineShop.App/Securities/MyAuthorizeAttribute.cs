using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShop.App.Securities
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                MyPrincipal mp = HttpContext.Current.User as MyPrincipal;
                if (!string.IsNullOrEmpty(Roles) && !mp.IsInRole(Roles))
                {
                    filterContext.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }
    }
}