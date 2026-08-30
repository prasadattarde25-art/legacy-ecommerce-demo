using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ecommerce.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AjaxValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            if (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var formToken = request.Form["__RequestVerificationToken"];
            var headerToken = request.Headers["__RequestVerificationToken"];

            bool valid = false;

            try
            {
                if (!string.IsNullOrEmpty(formToken))
                {
                    AntiForgery.Validate();
                    valid = true;
                }
                else if (!string.IsNullOrEmpty(headerToken))
                {
                    var parts = headerToken.Split(new[] { ':' }, 2);

                    if (parts.Length == 2)
                    {
                        AntiForgery.Validate(parts[0], parts[1]);
                    }
                    else
                    {
                        var cookie = request.Cookies["__RequestVerificationToken"]
                                     ?? request.Cookies.Cast<string>()
                                        .Select(n => request.Cookies[n])
                                        .FirstOrDefault(c => c != null && c.Name.StartsWith("__RequestVerificationToken", StringComparison.OrdinalIgnoreCase));

                        if (cookie != null)
                        {
                            AntiForgery.Validate(cookie.Value, headerToken);
                        }
                    }

                    valid = true;
                }
            }
            catch (HttpAntiForgeryException)
            {
                valid = false;
            }

            if (!valid)
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Anti-forgery token validation failed.");
            }
        }
    }
}