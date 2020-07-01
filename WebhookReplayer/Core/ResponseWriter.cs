using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace WebhookReplayer.Core
{
    public static class ResponseWriter
    {
        public static async Task<HttpResponse> Write(this HttpResponse response, HttpResponseMessage message)
        {
            response.StatusCode = (int) message.StatusCode;

            var headers = message.Headers
                .Concat(message.Content.Headers)
                .Where(m => !string.Equals(Headers.TRANSFER_ENCODING, m.Key,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var header in headers)
            {
                response.Headers.TryAdd(header.Key, header.Value.ToArray());
            }

            await response.Body.WriteAsync(await message.Content.ReadAsByteArrayAsync());

            return response;
        }
    }
}
