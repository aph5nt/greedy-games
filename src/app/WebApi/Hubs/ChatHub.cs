using System;
using System.Threading.Tasks;
using Akka.Actor;
using Chat.Contracts.Commands;
using Chat.Contracts.DataTransfer;
using Chat.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Providers;

namespace WebApi.Hubs
{
    [Authorize(AppPolicy.Default)]    
    public class ChatHub : Hub
    {
        private readonly IRemoteChatHubProvider _chatHubProvider;

        public ChatHub(IRemoteChatHubProvider chatHubProvider)
        {
            _chatHubProvider = chatHubProvider;
        }
 
        public override async Task OnConnectedAsync()
        {
            _chatHubProvider.Provide().Tell(new AddConnection(Context.ConnectionId));
            await ChangeUsersOnlineNumber();
            await SendArchiveMessages();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _chatHubProvider.Provide().Tell(new RemoveConnection(Context.ConnectionId));
            await ChangeUsersOnlineNumber();
        }

        public async Task SendMessage(string message)
        {
            var chatMessage = new ChatMessage {UserName = Context.User.Identity.Name, Message = message};
            _chatHubProvider.Provide().Tell(chatMessage);
            await Clients.All.InvokeAsync("onMessageSent", chatMessage);
        }


        private async Task SendArchiveMessages()
        {
            var chatArchive = await _chatHubProvider.Provide().Ask<ChatArchive>(new GetChatArchive(), TimeSpan.FromSeconds(30));
            await Clients.User(Context.User.Identity.Name).InvokeAsync("onArchiveMessagesSent", chatArchive);
        }
        
        
        private async Task ChangeUsersOnlineNumber()
        {
            var result = await _chatHubProvider.Provide().Ask<ConnectionCount>(new GetConnectionCount(), TimeSpan.FromSeconds(30));
            await Clients.All.InvokeAsync("onUsersOnlineNumberChange", new { result.Count });
        }
    }
}