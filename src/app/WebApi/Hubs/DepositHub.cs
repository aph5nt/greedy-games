using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Contracts.Events.Withdraws;

namespace WebApi.Hubs
{
    [Authorize(AppPolicy.Default)]     
    public class DepositHub : Hub
    {
        [Authorize(Policy = AppPolicy.SystemUser)]    
        public async Task Notify(IDepositNotification message)
        {
            await Clients.User(message.Identity.UserName).InvokeAsync("depositUpdated", message);
        }
    }
}