using Akka.Actor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Payments;
using Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Models;
using WebApi.Providers;
using WebApi.Services;
using Game.Minefield.Contracts.Services;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Models;
using WebApi.Helpers;
using WebApi.Infrastructure.Totp;

namespace WebApi.Controllers
{
    [Route("withdraw")]
    public class WithdrawController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IWithdrawService _withdrawService;
        private readonly IRemoteTransactionManagerProvider _remoteTransactionManagerProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebSettings _settings;

        public WithdrawController(IAccountService accountService, IWithdrawService withdrawService, IRemoteTransactionManagerProvider remoteTransactionManagerProvider, UserManager<ApplicationUser> userManager, WebSettings settings)
        {
            _accountService = accountService;
            _withdrawService = withdrawService;
            _remoteTransactionManagerProvider = remoteTransactionManagerProvider;
            _userManager = userManager;
            _settings = settings;
        }

        [HttpGet("history/{network}")]
        public async Task<IActionResult> GetDepositHistory(Network network, [FromQuery] int page = 1,  [FromQuery] int pageSize = 10)
        {
            if (network == Network.FREE)
            {
                return Forbidden("Network not supported.");
            }

            var result = await _withdrawService.GetAsync(network, User.Identity.Name, page, pageSize);
            
            return Ok(new
            {
                Data = result.ToArray(),
                TotalPages = result.TotalPages,
                PageIndex = result.PageIndex,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage
            });
        }
              
        [HttpGet("bankroll/history/{network}")]
        public async Task<IActionResult> GetBankrollDepositHistory(Network network, [FromQuery] int page = 1,  [FromQuery] int pageSize = 10)
        {
            var result = await _withdrawService.GetAsync(network, GameTypes.Minefield.ToString(), page, pageSize);
            
            return Ok(new
            {
                Data = result.ToArray(),
                TotalPages = result.TotalPages,
                PageIndex = result.PageIndex,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage
            });
        }
         
        [HttpPost]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawFormModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return Unauthorized();
            }
 
            if (!new TotpValidator(new TotpGenerator()).Validate(user.TwoFactorAuthSecret, model.TwoFactorAuthCode))
            {
                return Unauthorized();
            }

            if (model.Amount < _settings.Withdraw.MinAmount || model.Amount >= _settings.Withdraw.MaxAmount)
            {
                return UnProcessableEntity(nameof(model.Amount), "Invalid amount.");
            }

            var balance = await _remoteTransactionManagerProvider.Provide().Ask<Balance>(new GetBalance(model.Network, User.Identity.Name));
            if (balance.Amount < model.Amount)
            {
                return UnProcessableEntity(nameof(model.Amount), "No funds.");
            }

            await _withdrawService.WithdrawAsync(
                User.Identity.Name,
                model.Network, 
                model.DestinationAddress,
                model.Amount);

            return Created(string.Empty, null);
        }
    }
}