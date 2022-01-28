using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Payments;
using Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    [Authorize]
    [Route("deposit")]
    public class DepositController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IDepositService _depositService;

        public DepositController(IAccountService accountService, IDepositService depositService)
        {
            _accountService = accountService;
            _depositService = depositService;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> Index(Network network)
        {
            var userAccount = await _accountService.GetAsync(network, User.Identity.Name);
            
            return View(new DepositViewModel
            {
                UserAccount = userAccount,
                Deposits = userAccount.IsActive ? await _depositService.GetAsync(network, User.Identity.Name) : new List<Deposit>()
            });
        }

        [HttpPost("{network}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateNetwork(Network network)
        {
            if (ModelState.IsValid)
            {
                await _accountService.ActivateAsync(network, User.Identity.Name);
                return RedirectToAction(nameof(Index), new { network });
            }

            var userAccount = await _accountService.GetAsync(network, User.Identity.Name);
            var viewModel = new DepositViewModel
            {
                UserAccount = userAccount,
                Deposits = userAccount.IsActive ? await _depositService.GetAsync(network, User.Identity.Name) : new List<Deposit>()
            };

            return View("Index", viewModel);
        }
    }
}