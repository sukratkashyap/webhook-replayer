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

        public async Task Invoke(HttpContext context, IProxyHttpClient client)
        {
            var request = context.Request;
            var response = context.Response;

            var proxyRequest = CreateProxyRequest(request);
            if (!request.Headers.ContainsKey("X-Test"))
            {
                var proxyResponse = await client.SendAsync(proxyRequest);
                response.StatusCode = (int) proxyResponse.StatusCode;
                var headers = proxyResponse.Headers.Union(proxyResponse.Content.Headers);
                foreach (var header in headers)
                {
                    if (string.Equals(header.Key, "Transfer-Encoding", StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    response.Headers.TryAdd(header.Key, new StringValues(header.Value.ToArray()));
                }

                var body = await proxyResponse.Content.ReadAsByteArrayAsync();
                await response.Body.WriteAsync(body);
            }
            else
            {
                response.StatusCode = 200;
                await response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Method = proxyRequest.Method.Method,
                    Url = proxyRequest.RequestUri,
                    Headers = proxyRequest.Headers.ToDictionary(m => m.Key, m => m.Value),
                }));
            }
        }

        private HttpRequestMessage CreateProxyRequest(HttpRequest request)
        {
            if (!request.Query.ContainsKey("_to"))
            {
                throw new Exception("Query parameter _to=URL is missing!");
            }

            var location = HttpUtility.UrlDecode(request.Query["_to"].FirstOrDefault());
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new Exception("Query parameter _to is null or empty!");
            }

            location = location.TrimEnd('/');
            var path = request.Path.Value?.Trim('/');
            var queryList = request.Query
                .Where(m => !string.Equals(m.Key, "_to", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(m => m.Value.Select(v => KeyValuePair.Create(m.Key, v)))
                .ToList();
            var queryString = string.Join("&", queryList.Select(m => $"{m.Key}={HttpUtility.UrlEncode(m.Value)}"));

            var headers = request.Headers
                .Where(m => !string.Equals(m.Key, "Host", StringComparison.InvariantCultureIgnoreCase))
                .Where(m => !string.Equals(m.Key, "Transfer-Encoding", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var uri = CreateUri($"{location}/{path}?{queryString}");
            var result = new HttpRequestMessage();
            result.Method = new HttpMethod(request.Method);
            result.RequestUri = uri;
            result.Content = new StreamContent(request.Body);
            foreach (var header in headers)
            {
                result.Headers.TryAddWithoutValidation(header.Key, header.Value.ToList());
            }

            return result;
        }

        private Uri CreateUri(string url)
        {
            try
            {
                return new Uri(url);
            }
            catch (UriFormatException)
            {
                throw new UriFormatException($"Invalid URI: {url}");
            }
        }
    }
}
