using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebhookReplayer.Core
{
    public class LinuxProxyHttpClient : IProxyHttpClient
    {
        private HttpClient _client;

        public LinuxProxyHttpClient()
        {
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return _client.SendAsync(requestMessage);
        }
    }
}
