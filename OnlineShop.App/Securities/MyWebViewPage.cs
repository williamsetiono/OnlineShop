using System.Web.Mvc;

namespace OnlineShop.App.Securities
{
    public abstract class MyWebViewPage : WebViewPage
    {
        public virtual new MyPrincipal User
        {
            get { return base.User as MyPrincipal; }
        }
    }

    public abstract class MyWebViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new MyPrincipal User
        {
            get { return base.User as MyPrincipal; }
        }
    }

}