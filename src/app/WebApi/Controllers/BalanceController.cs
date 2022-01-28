using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Models;
using Shared.Model;
using WebApi.Providers;

namespace WebApi.Controllers
{
    [Route("balance")]
    public class BalanceController : BaseApiController
    {
        private readonly IRemoteTransactionManagerProvider _remoteTransactionManagerProvider;

        public BalanceController(IRemoteTransactionManagerProvider remoteTransactionManagerProvider)
        {
            _remoteTransactionManagerProvider = remoteTransactionManagerProvider;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> GetUserBalance(Network network)
        {
            var balance = await _remoteTransactionManagerProvider.Provide().Ask<Balance>(new GetBalance(network, User.Identity.Name), TimeSpan.FromSeconds(30));
            return Ok(balance);
        }
        
        [HttpGet("bankroll/{network}")]
        public async Task<IActionResult> GetBankrollBalance(Network network)
        {
            var balance = await _remoteTransactionManagerProvider.Provide().Ask<Balance>(new GetBalance(network, GameTypes.Minefield.ToString()), TimeSpan.FromSeconds(30));
            return Ok(balance);
        }
    }
}