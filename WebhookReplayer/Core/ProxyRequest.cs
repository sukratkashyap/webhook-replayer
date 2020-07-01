using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace WebhookReplayer.Core
{
    public class ProxyRequest
    {
        public const string TO_PARAM = "_to";

        public async Task<HttpRequestMessage> Create(HttpRequest request)
        {
            var method = new HttpMethod(request.Method);
            var url = ConstructUrl(request);
            var headers = ConstructHeaders(request);
            var stream = new StreamContent(request.Body);
            var content = await stream.ReadAsByteArrayAsync();

            var result = new HttpRequestMessage
            {
                RequestUri = url,
                Method = method,
                Content = new ByteArrayContent(content)
            };
            foreach (var header in headers)
            {
                result.Headers.TryAddWithoutValidation(header.Key, header.Value.ToList());
            }

            return result;
        }

        private Dictionary<string, StringValues> ConstructHeaders(HttpRequest request)
        {
            var headers = request.Headers
                .Where(m => !string.Equals(Headers.HOST, m.Key, StringComparison.OrdinalIgnoreCase))
                .Where(m => !string.Equals(Headers.TRANSFER_ENCODING, m.Key,
                    StringComparison.OrdinalIgnoreCase))
                .Where(m => !string.Equals(Headers.SHOW_PROXY_REQUEST, m.Key,
                    StringComparison.OrdinalIgnoreCase))
                .Where(m => !string.Equals(Headers.SHOW_PROXY_RESPONSE, m.Key,
                    StringComparison.OrdinalIgnoreCase))
                .ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
            if (!headers.ContainsKey(Headers.CONTENT_TYPE))
            {
                headers.Add(Headers.CONTENT_TYPE, request.ContentType);
            }

            return headers;
        }

        private Uri ConstructUrl(HttpRequest request)
        {
            if (!request.Query.ContainsKey(TO_PARAM))
            {
                throw new InvalidOperationException(
                    "Query parameter _to=URL is missing!");
            }

            var baseUrl = HttpUtility.UrlDecode(request.Query["_to"].FirstOrDefault());
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Query parameter _to is null or empty!");
            }

            var uriBuilder = CreateUriBuilder(baseUrl);
            if (request.Path.HasValue)
            {
                uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + request.Path.Value;
            }

            var queryList = QueryHelpers.ParseQuery(uriBuilder.Query)
                .SelectMany(m => m.Value.Select(o => KeyValuePair.Create(m.Key, o)))
                .ToList();
            if (request.QueryString.HasValue)
            {
                var requestQuery = request.Query
                    .SelectMany(m => m.Value.Select(o => KeyValuePair.Create(m.Key, o)))
                    .Where(m => !string.Equals(TO_PARAM, m.Key, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                queryList.AddRange(requestQuery);
            }

            uriBuilder.Query = string.Join("&", queryList
                .Select(m => $"{HttpUtility.UrlEncode(m.Key)}={HttpUtility.UrlEncode(m.Value)}"));
            return
                uriBuilder.Uri;
        }

        private UriBuilder CreateUriBuilder(string url)
        {
            try
            {
                return new UriBuilder(url);
            }
            catch (UriFormatException)
            {
                throw new UriFormatException($"Invalid URI in _to parameter: {url}");
            }
        }
    }
}
