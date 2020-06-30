using Microsoft.AspNetCore.Builder;

namespace WebhookReplayer.Middleware
{
    public static class ReplayerMiddlewareExtension
    {
        public static IApplicationBuilder UseReplayerMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReplayerMiddleware>();
        }
    }
}
