using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{

    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly WebSettings _settings;

        public AccountController(
            IAccountService accountService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            WebSettings settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = settings;
            _accountService = accountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
         
        [AllowAnonymous]
        [HttpGet("{userName}/{password}", Name = "Login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, lockoutOnFailure: false);
            return RedirectToLocal("/");
        }

        [AllowAnonymous]
        [HttpPost]
        public  IActionResult  GenerateToken()
        {
            var user = User.Identity;

            if (user != null && user.IsAuthenticated)
            {
                    var options = new IdentityOptions();
                    var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    var issueTime = DateTime.Now;
                    var iat = (long)issueTime.Subtract(utc0).TotalSeconds;

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Aud, "api"),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                        new Claim(options.ClaimsIdentity.UserNameClaimType, user.Name),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, iat.ToString(), ClaimValueTypes.Integer64),
                    };
                
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Tokens.Key));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _settings.Tokens.Issuer,
                    "api",
                    claims,
                    expires: issueTime.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                
            }

            return BadRequest(new { error = "Could not create token" });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToLocal("/");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateRecaptcha]
        public async Task<IActionResult> Register(object model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var credentials = await _accountService.CreateAsync();
                UserName = credentials.userName;
                Password = credentials.password;
                AuthUrl = Url.Link("Login", new { credentials.userName,  credentials.password});

                return RedirectToAction(nameof(Created));
            }

            return View(model);
        }

        [TempData]
        public string UserName { get; set; }

        [TempData]
        public string Password { get; set; }

        [TempData]
        public string AuthUrl { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Created()
        {
            ViewBag.UserName = UserName;
            ViewBag.Password = Password;
            ViewBag.AuthUrl = AuthUrl;

            UserName = null;
            Password = null;
            AuthUrl = null;

            return View(nameof(Created));
        }


        [TempData]
        public string ErrorMessage { get; set; }

        #region Helpers
 
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
