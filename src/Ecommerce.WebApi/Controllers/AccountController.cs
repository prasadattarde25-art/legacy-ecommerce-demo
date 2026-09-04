using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.WebApi.Services;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accounts;
        private readonly TokenService _tokens;

        public AccountController(IAccountService accounts, TokenService tokens)
        {
            _accounts = accounts;
            _tokens = tokens;
        }

        /// <summary>Authenticates a customer and returns a JWT.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct = default)
        {
            var result = await _accounts.LoginAsync(model, ct);
            return ToAuthResult(result);
        }

        /// <summary>Creates a customer account and returns a JWT.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct = default)
        {
            var result = await _accounts.RegisterAsync(model, ct);
            return ToAuthResult(result);
        }

        /// <summary>Logs the current user out. JWT is stateless — the client drops the token.</summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { success = true });
        }

        /// <summary>Order history for the signed-in customer.</summary>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> Orders(CancellationToken ct = default)
        {
            var customerId = GetCustomerId();
            if (customerId <= 0) return Unauthorized();
            var vm = await _accounts.GetOrderHistoryAsync(customerId, ct);
            return Ok(vm);
        }

        private IActionResult ToAuthResult(ServiceResult<Customer> result)
        {
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            var customer = result.Value;
            return Ok(new AuthResponse
            {
                success = true,
                token = _tokens.CreateToken(customer),
                customer = new
                {
                    customer.Id,
                    customer.Email,
                    customer.FirstName,
                    customer.LastName,
                    name = (customer.FirstName + " " + customer.LastName).Trim()
                }
            });
        }

        private int GetCustomerId()
        {
            var principal = User as ClaimsPrincipal;
            var claim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            int id;
            return claim != null && int.TryParse(claim.Value, out id) ? id : 0;
        }

        public class AuthResponse
        {
            public bool success { get; set; }
            public string token { get; set; }
            public object customer { get; set; }
        }
    }
}
