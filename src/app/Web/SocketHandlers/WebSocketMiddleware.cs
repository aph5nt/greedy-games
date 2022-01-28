using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Web.SocketHandlers
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _serverToken;
        private WebSocketHandler WebSocketHandler { get; set; }

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler, string serverToken)
        {
            _next = next;
            _serverToken = serverToken;
            WebSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var receivedServerToken = context.Request.Headers["serverToken"];

                if (!String.IsNullOrEmpty(receivedServerToken) && receivedServerToken == _serverToken)
                {
                    await AddConnection(context);
                }
                else if (context.User.Identity.IsAuthenticated)
                {
                    await AddConnection(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                }
            }
        }

        private async Task AddConnection(HttpContext context)
        {
            var connection = await WebSocketHandler.OnConnected(context);
            if (connection != null)
            {
                await WebSocketHandler.ListenConnection(connection);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
