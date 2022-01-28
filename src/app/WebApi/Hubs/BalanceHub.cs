using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Contracts.Models;
using Shared.Model;

namespace WebApi.Hubs
{
    [Authorize(AppPolicy.Default)]    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BalancesHub : Hub
    {
        [Authorize(AppPolicy.SystemUser)]    
        public async Task Notify(Balance message)
        {
            await Clients.User(message.UserName).InvokeAsync("onBalanceUpdated", message);
        }
    }
}
