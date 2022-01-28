using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Game.Minefield.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Payment.Messages.Commands.Balaces;
using Payment.Messages.Models;
using Shared.Model;
using Web.Providers;
using IBalanceProvider = Web.Providers.IBalanceProvider;

namespace Web.Views.Shared.Components.Navigation
{
    public class NavigationViewModel
    {
        public NavigationViewModel()
        {
            Networks = Enum.GetValues(typeof(Network)).Cast<Network>().ToArray();
        }

        public Network[] Networks { get; }
        public Network SelectedNetwork { get; set; }
        public long Balance { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Curent { get; set; }
        public string SelectedNetworkStr { get; set; }
    }

    public class Navigation : ViewComponent
    {
        private readonly IBalanceProvider _balanceProvider;

        public Navigation(IBalanceProvider balanceProvider)
        {
            _balanceProvider = balanceProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync(string controllerName)
        {
            var viewModel = new NavigationViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
            };

            var networkObj = RouteData.Values["network"];
            viewModel.SelectedNetwork = 
                networkObj != null ? 
                Enum.Parse<Network>(networkObj.ToString(), true) : Network.FREE;

            var networkSymbol = viewModel.SelectedNetwork.ToString().ToUpper();
            viewModel.SelectedNetworkStr = viewModel.SelectedNetwork != Network.FREE
                ? Money.CentPrefix + networkSymbol
                : networkSymbol;

            if (User.Identity.IsAuthenticated)
            {
                var amount = _balanceProvider.Provide(viewModel.SelectedNetwork).Ask<Balance>(new GetBalance(viewModel.SelectedNetwork, User.Identity.Name)).Result.Amount;
                viewModel.Balance = (long)(amount / Money.Sathoshi);
            }
            
            viewModel.Curent = controllerName != "Home" ? controllerName : "Game";

            return View("Default", viewModel);
        }
    }
}
