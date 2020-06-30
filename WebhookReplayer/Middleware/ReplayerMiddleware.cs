using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace WebhookReplayer.Middleware
{
    public class ReplayerMiddleware
    {
        private readonly RequestDelegate _next;

        public ReplayerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Hello");
        }
    }
}
