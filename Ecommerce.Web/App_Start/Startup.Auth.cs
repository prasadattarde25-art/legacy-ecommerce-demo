using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Ecommerce.Web
{
    public partial class Startup
    {
        public const string AuthenticationType = "ApplicationCookie";

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = AuthenticationType,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromDays(7),
                SlidingExpiration = true,
                CookieHttpOnly = true,
                CookieSameSite = SameSiteMode.Lax
            });
        }
    }
}