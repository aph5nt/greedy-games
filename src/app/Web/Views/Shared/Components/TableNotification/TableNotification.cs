using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System;
using System.Threading.Tasks;

namespace Web.Views.Shared.Components.TableNotification
{
    public class TableNotificationViewModel
    {
        public Network Network { get; set; }
        public bool IsActivated { get; set; }
        public string HubName { get; set; }
    }

    public class TableNotification : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string hubName, bool isActivated)
        {
            var viewModel = new TableNotificationViewModel
            {
                IsActivated = isActivated,
                HubName = hubName
            };

            var networkObj = RouteData.Values["network"];
            viewModel.Network =
                networkObj != null ?
                    Enum.Parse<Network>(networkObj.ToString(), true) : Network.FREE;
           
            return View("Default", viewModel);
        }
    }
}
