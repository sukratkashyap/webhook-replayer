using System.Net.Http;
using System.Threading.Tasks;

namespace WebhookReplayer.Core
{
    public class WindowsProxyHttpClient : IProxyHttpClient
    {
        private HttpClient _client;

        public WindowsProxyHttpClient()
        {
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return _client.SendAsync(requestMessage);
        }
    }
}
