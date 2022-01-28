using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Infrastructure.Totp;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("2fa")]
    public class TwoFactorAuthController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public TwoFactorAuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet("generate")]
        public async Task<IActionResult> Generate2FaqrCode()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var response = new TotpSetupGenerator().Generate(
                "GreedyGames",
                User.Identity.Name,
                user.TwoFactorAuthSecret,
                qrCodeHeight: 220);
            
            return Ok(response);
        }
        
        [HttpPut("enable")]
        public async Task<IActionResult> Enable([FromBody]TwoFactorAuthCode body)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var isValid = new TotpValidator(new TotpGenerator()).Validate(user.TwoFactorAuthSecret, body.Value);

            if (isValid)
            {
                user.TwoFactorAuthEnabled = true;
                await _userManager.UpdateAsync(user);

                return NoContent();
            }
            
            return UnProcessableEntity(nameof(body.Value), "Provided auth code is invalid."); 
        }
        
        [HttpPut("disable")]
        public async Task<IActionResult> Disable([FromBody]TwoFactorAuthCode body)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var isValid = new TotpValidator(new TotpGenerator()).Validate(user.TwoFactorAuthSecret, body.Value);
            if (isValid)
            {

                user.TwoFactorAuthEnabled = false;
                user.TwoFactorAuthSecret = SecurityHelper.CreateRandomPassword(8);
                await _userManager.UpdateAsync(user);

                return NoContent();
            }

            return UnProcessableEntity(nameof(body.Value), "Provided auth code is invalid.");
        }
        
        
        [HttpGet("")]
        public async Task<IActionResult> Get2Fa()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            return Ok(new
            {
                enabled = user.TwoFactorAuthEnabled,
            });
        }
    }
}