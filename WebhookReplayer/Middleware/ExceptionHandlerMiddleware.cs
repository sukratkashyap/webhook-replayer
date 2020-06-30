using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebhookReplayer.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.StatusCode = 500;
                response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");
                await response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Type = ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace
                }, Formatting.Indented));
            }
        }
    }
}
