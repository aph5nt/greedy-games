using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Payment.Contracts.Events.Withdraws;

namespace WebApi.Hubs
{
    [Authorize(AppPolicy.Default)]       
    public class WithdrawHub : Hub
    {
        [Authorize(Policy = AppPolicy.SystemUser)]    
        public async Task Notify(IWithdrawNotification message)
        {
            await Clients.User(message.Identity.UserName).InvokeAsync("withdrawUpdated", message);
        }
    }
}