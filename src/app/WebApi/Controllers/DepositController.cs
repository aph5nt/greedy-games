using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Payments;
using Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("deposit")]
    public class DepositController : BaseApiController
    {
        private readonly IDepositService _depositService;

        public DepositController(IAccountService accountService, IDepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpGet("history/{network}")]
        public async Task<IActionResult> GetDepositHistory(Network network, [FromQuery] int page = 1,  [FromQuery] int pageSize = 10)
        {
            if (network == Network.FREE)
            {
                return Forbidden("Network not supported.");
            }

            var result = await _depositService.GetAsync(network, User.Identity.Name, page, pageSize);
            
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
            var result = await _depositService.GetAsync(network, GameTypes.Minefield.ToString(), page, pageSize);
            
            return Ok(new
            {
                Data = result.ToArray(),
                TotalPages = result.TotalPages,
                PageIndex = result.PageIndex,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage
            });
        }
        
    }
}