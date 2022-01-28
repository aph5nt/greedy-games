using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace WebApi.Infrastructure
{
    #region

    #endregion

    public class SimpleCorsMiddleware
    {
        private readonly RequestDelegate _next;

        private IHostingEnvironment _environment;

        public SimpleCorsMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            this._next = next;
            this._environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(async () =>
            {
                try
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
                    context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept, Authorization" });
                    context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("OK");
                    }
                    else
                    {
                        await this._next(context);
                    }
                
                    await this._next(context);
                
                }
                catch (Exception)
                {
                    await this._next(context);
                }
            });
            
            
        }
    }

    public static class OptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleCors(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SimpleCorsMiddleware>();
        }
    }
}