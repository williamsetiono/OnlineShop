using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineShop.App.Startup))]
namespace OnlineShop.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
