using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Shared.Model;
using WebApi.Configuration;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("account")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IReCaptchaValidation _reCaptchaValidation;
        
        public AccountController(
            IAccountService accountService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IReCaptchaValidation reCaptchaValidation,
            WebSettings settings)
        {
            _accountService = accountService;
            _reCaptchaValidation = reCaptchaValidation;
        }
 
        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (_reCaptchaValidation.VerifyCaptcha(model.Token))
            {
                var credentials = await _accountService.CreateAsync();
                return Created(string.Empty, new { credentials.UserName, credentials.Password });
            }

            return BadRequest();
        }
        
        [AllowAnonymous]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            return Ok(new UserCount
            {
                Count = await _accountService.CountAsync()
            });
        }
        
        [AllowAnonymous]
        [HttpGet("generatepassword")]
        public IActionResult GeneratePassword()
        {
            return Ok(new 
            {
                Password = SecurityHelper.CreateRandomPassword(8)
            });
        }
        
        [HttpGet("{network}")]
        public async Task<IActionResult> GetAccount(Network network)
        {
            return Ok(await _accountService.GetUserAccountAsync(network, User.Identity.Name));
        }
        
        [HttpGet("bankroll/{network}")]
        public async Task<IActionResult> GetBankrollAccount(Network network)
        {
            
            return Ok(await _accountService.GetGameAccountAsync(network, GameTypes.Minefield.ToString()));
        }

        [HttpPut("{network}")]
        public async Task<IActionResult> ActivateNetwork(Network network)
        {
            if (network == Network.FREE)
            {
                return Forbidden("Network not supported.");
            }

            await _accountService.ActivateAsync(network, User.Identity.Name);
            return NoContent();
        }
        
    }

    public class TwoFactorAuthCode
    {
        public int Value { get; set; }
    }
}
