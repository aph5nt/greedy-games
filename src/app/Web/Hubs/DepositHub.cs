using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Messages.Events.Withdraws;

namespace Web.Hubs
{
    [Authorize(Policy = "CanUseHubs")]
    public class DepositHub : Hub
    {
        public async Task Notify(IDepositNotification message)
        {
            await Clients.User(message.Identity.UserName).InvokeAsync("depositUpdated", message);
        }
    }
}