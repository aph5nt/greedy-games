using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Messages.Events.Withdraws;

namespace Web.Hubs
{
    [Authorize(Policy = "CanUseHubs")]
    public class WithdrawHub : Hub
    {
        public async Task Notify(IWithdrawNotification message)
        {
            await Clients.User(message.Identity.UserName).InvokeAsync("withdrawUpdated", message);
        }
    }
}