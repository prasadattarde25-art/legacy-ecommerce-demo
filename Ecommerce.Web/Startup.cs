using Microsoft.Owin;
using Owin;
using Ecommerce.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace Ecommerce.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}