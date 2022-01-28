using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Web.SocketHandlers
{
    public static class Extensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler, string serverToken)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketMiddleware>(handler, serverToken));
        }
    }
}
