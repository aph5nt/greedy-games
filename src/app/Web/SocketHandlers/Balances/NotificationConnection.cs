using Newtonsoft.Json;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Payment.Messages.Models;

namespace Web.SocketHandlers.Balances
{
    public class NotificationConnection : DefaultConnection
    {
        private readonly WebSocketHandler _handler;

        public NotificationConnection(WebSocket webSocket, WebSocketHandler handler) : base(webSocket)
        {
            _handler = handler;
        }

        public override async Task ReceiveAsync(string message)
        {
            var notification = JsonConvert.DeserializeObject<Notification>(message);
            if (notification != null)
            {
                var receivers = _handler
                    .Connections
                    .Where(m => m.WebSocket.State == WebSocketState.Open &&
                                m.Type == notification.Type &&
                                m.Network == notification.Network &&
                                m.UserName == notification.UserName)
                    .ToList();

                foreach (var receiver in receivers)
                {
                    try
                    {
                        await receiver.SendMessageAsync(message);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }
    }
}