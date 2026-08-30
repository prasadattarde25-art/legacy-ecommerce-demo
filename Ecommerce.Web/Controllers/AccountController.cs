using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accounts;

        public AccountController(IAccountService accounts)
        {
            _accounts = accounts;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            ViewBag.Title = "Log In";
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            ViewBag.Title = "Log In";

            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(model);

            var result = _accounts.Login(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            SignIn(result.Value, model.RememberMe);
            return SafeRedirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(Startup.AuthenticationType);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            ViewBag.Title = "Register";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ViewBag.Title = "Register";

            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(model);

            var result = _accounts.Register(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            SignIn(result.Value, false);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Orders()
        {
            ViewBag.Title = "Order History";
            var vm = _accounts.GetOrderHistory(GetCustomerId());
            return View(vm);
        }

        private void SignIn(Customer customer, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Name, (customer.FirstName + " " + customer.LastName).Trim())
            }, Startup.AuthenticationType);

            var properties = new Microsoft.Owin.Security.AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            HttpContext.GetOwinContext().Authentication.SignIn(properties, identity);
        }

        private int GetCustomerId()
        {
            var principal = User as ClaimsPrincipal;
            string value = null;
            if (principal != null)
            {
                var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null) value = claim.Value;
            }

            int id;
            int.TryParse(value, out id);
            return id;
        }

        private ActionResult SafeRedirect(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}