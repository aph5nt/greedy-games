using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Messages.Models;
using System.Threading.Tasks;
using Shared.Model;

namespace Web.Hubs
{
    [Authorize(Policy = "CanUseHubs")]
    public class BalanceHub : Hub
    {
        public async Task Notify(Balance message)
        {
            message.Amount = Money.SatoshiToCent(message.Amount);
            await Clients.User(message.UserName).InvokeAsync("balanceUpdated", message);
        }
    }
}
