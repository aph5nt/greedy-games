using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Payment.Messages.Models;
using Shared.Model;
using Web.SocketHandlers.Balances;

namespace Web.SocketHandlers
{
    public abstract class DefaultConnection
    {
        public WebSocket WebSocket { get; }

        public string UserName { get; set; }
        public Network Network { get; set; }
        public string SessionId { get; set; }
        public NotificationType Type { get; set; }

        protected DefaultConnection(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public virtual async Task SendMessageAsync(string message)
        {
            if (WebSocket.State != WebSocketState.Open)
            {
                return;
            }

            var arr = Encoding.UTF8.GetBytes(message);

            var buffer = new ArraySegment<byte>(
                array: arr,
                offset: 0,
                count: arr.Length);

            await WebSocket.SendAsync(
                buffer: buffer,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None
            );
        }

        public abstract Task ReceiveAsync(string message);
    }
}