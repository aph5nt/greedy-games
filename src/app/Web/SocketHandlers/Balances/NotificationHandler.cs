using Microsoft.AspNetCore.Http;
using Shared.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Payment.Messages.Models;

namespace Web.SocketHandlers.Balances
{
    public class NotificationHandler : WebSocketHandler
    {
        private readonly string _serverToken;
        protected override int BufferSize => 8192;

        public NotificationHandler(string serverToken)
        {
            _serverToken = serverToken;
        }

        public override async Task<DefaultConnection> OnConnected(HttpContext context)
        {
            string serverToken = context.Request.Headers["serverToken"];
            string sessionId = context.Request.Query["sessionId"];
            string userName = context.Request.Query["userName"];
            string networkArg = context.Request.Query["network"];
            string typeArg = context.Request.Query["type"];

            var network = networkArg != null
                ? (Network) Enum.Parse(typeof(Network), context.Request.Query["network"], true)
                : Network.FREE;

            var type = typeArg != null 
                ? (NotificationType)Enum.Parse(typeof(NotificationType), context.Request.Query["type"], true) :
                NotificationType.Server;

            if (!String.IsNullOrEmpty(serverToken))
            {
                if (_serverToken != serverToken)
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                var connection = Connections.FirstOrDefault(m => m.SessionId == sessionId);

                if (connection == null)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    connection = new NotificationConnection(webSocket, this)
                    {
                        SessionId = sessionId,
                        UserName = userName,
                        Network = network,
                        Type = type
                    };

                    Connections.Add(connection);
                }

                return connection;
            }

            return null;
        }
    }
}
