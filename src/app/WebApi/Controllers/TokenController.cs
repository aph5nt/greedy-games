using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configuration;
using WebApi.Infrastructure.Totp;
using WebApi.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApi.Controllers
{
    [Route("token")]
    public class TokenController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly WebSettings _settings;

        public TokenController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            WebSettings settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _settings = settings;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                if (user.TwoFactorAuthEnabled)
                {
                    var isValid = new TotpValidator(new TotpGenerator()).Validate(user.TwoFactorAuthSecret, model.TwoFactorAuthCode);
                    if (!isValid)
                    {
                        return BadRequest(new { error = "Could not create token" });
                    }
                }
                
                
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var options = new IdentityOptions();
                    var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    var issueTime = DateTime.Now;
                    var iat = (long)issueTime.Subtract(utc0).TotalSeconds;

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Aud, _settings.Tokens.Audience),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, iat.ToString(), ClaimValueTypes.Integer64),
                        new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id),
                        new Claim(options.ClaimsIdentity.UserNameClaimType, user.UserName)
                    };
                    
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var userRole in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Tokens.Key));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _settings.Tokens.Issuer,
                        _settings.Tokens.Audience,
                        claims,
                        expires: null,//DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: creds);

                    return Ok(
                        new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            userName = model.UserName
                        });
                }
            }

            return BadRequest(new { error = "Could not create token" });
        }
    }
}