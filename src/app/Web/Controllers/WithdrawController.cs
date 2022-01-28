using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Identity;
using Shared.Model;
using Web.Models;
using Web.Providers;
using Web.Services;
using Payment.Contracts.Models;

namespace Web.Controllers
{
    [Authorize]
    [Route("withdraw")]
    public class WithdrawController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWithdrawService _withdrawService;
        private readonly IBalanceProvider _balanceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public WithdrawController(IAccountService accountService, IWithdrawService withdrawService, IBalanceProvider balanceProvider, UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _withdrawService = withdrawService;
            _balanceProvider = balanceProvider;
            _userManager = userManager;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> Index(Network network)
        {
            var userAccount = await _accountService.GetAsync(network, User.Identity.Name);

            if (!userAccount.IsActive)
            {
                return RedirectToAction("Index", "Deposit");
            }

            return View(new WithdrawViewModel
            {
                Form = new WithdrawFormViewModel
                {
                    Network = network
                },
                UserAccount = userAccount,
                Withdraws = userAccount.IsActive ? await _withdrawService.GetAsync(network, User.Identity.Name) : new List<UserWithdraw>()
            });
        }

        [HttpPost("{network}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Network network, [FromForm] WithdrawViewModel model)
        {
            var isPasswordValid = await _userManager.CheckPasswordAsync(
                await _userManager.FindByNameAsync(User.Identity.Name),
                model.Form.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("Password", "Provided password is invalid.");
            }

            if (ModelState.IsValid)
            {
                await _withdrawService.WithdrawAsync(
                    User.Identity.Name,
                    network,
                    model.Form.DestinationAddress,
                    (long) (model.Form.Amount * Money.Sathoshi));

                return RedirectToAction("Submited");
            }

            return View("Index");
        }

        [HttpGet("submited")]
        public IActionResult Submited(Network network)
        {
            return View();
        }

        public async Task<IActionResult> ValidateAmount(decimal amount, Network network)
        {
            var balance = await _balanceProvider.Provide(network).Ask<Balance>(new GetBalance(network, User.Identity.Name));
            var money = (long) amount * Money.Sathoshi;

            if (balance.Amount < money)
            {
                return BadRequest(new {error = "No funds"});
            }

            return Json("true");
        }
    }
}
