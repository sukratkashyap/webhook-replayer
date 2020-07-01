using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog;
using WebhookReplayer.Core;

namespace WebhookReplayer.Middleware
{
    public class ReplayerMiddleware
    {
        private readonly RequestDelegate _next;

        public ReplayerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IProxyHttpClient client, ProxyRequest proxyRequest)
        {
            var request = context.Request;
            var response = context.Response;

            var requestMessage = await proxyRequest.Create(request);
            if (request.Headers.ContainsKey(Headers.SHOW_PROXY_REQUEST))
            {
                response.StatusCode = 200;
                var requestModel = await RequestModel.Create(requestMessage);
                await response.WriteAsync(JsonConvert.SerializeObject(requestModel, Formatting.Indented));
                return;
            }

            var proxyResponse = await client.SendAsync(requestMessage);

            if (request.Headers.ContainsKey(Headers.SHOW_PROXY_RESPONSE))
            {
                response.StatusCode = 200;
                var requestModel = await RequestModel.Create(requestMessage);
                var responseModel = await ResponseModel.Create(proxyResponse);
                await response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Request = requestModel,
                    Response = responseModel
                }, Formatting.Indented));
                return;
            }

            await response.Write(proxyResponse);
        }
    }
}
