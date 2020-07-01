using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace WebhookReplayer.Core
{
    public class RequestModel
    {
        public string Method { get; set; }

        public string Url { get; set; }

        public Dictionary<string, IEnumerable<string>> Headers { get; set; }

        public string Body { get; set; }

        public static async Task<RequestModel> Create(HttpRequestMessage request)
        {
            var mainHeaders = request.Headers.ToDictionary(m => m.Key, m => m.Value);
            var contentHeaders = request.Content.Headers.ToDictionary(m => m.Key, m => m.Value);
            return new RequestModel
            {
                Method = request.Method.Method,
                Url = request.RequestUri.ToString(),
                Headers = mainHeaders.Union(contentHeaders).ToDictionary(m => m.Key, m => m.Value),
                Body = await request.Content.ReadAsStringAsync()
            };
        }
    }

    public class ResponseModel
    {
        public int StatusCode { get; set; }

        public string Url { get; set; }

        public Dictionary<string, IEnumerable<string>> Headers { get; set; }

        public string Body { get; set; }

        public static async Task<ResponseModel> Create(HttpResponseMessage response)
        {
            var mainHeaders = response.Headers.ToDictionary(m => m.Key, m => m.Value);
            var contentHeaders = response.Content.Headers.ToDictionary(m => m.Key, m => m.Value);
            return new ResponseModel()
            {
                StatusCode = (int) response.StatusCode,
                Headers = mainHeaders.Union(contentHeaders).ToDictionary(m => m.Key, m => m.Value),
                Body = await response.Content.ReadAsStringAsync()
            };
        }
    }
}
