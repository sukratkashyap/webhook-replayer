using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WebhookReplayer.Core
{
    public class WindowsProxyHttpClient : IProxyHttpClient
    {
        private HttpClient _client;

        public WindowsProxyHttpClient(Settings settings)
        {
            _client = CreateClient(settings);
        }

        public static HttpClient CreateClient(Settings settings)
        {
            var httpClientHandler = new HttpClientHandler();
            string proxyUrl = string.IsNullOrWhiteSpace(settings.HttpProxy) ? settings.HttpsProxy : settings.HttpProxy;
            if (!string.IsNullOrWhiteSpace(proxyUrl))
            {
                var uri = new UriBuilder(proxyUrl);
                var proxy = new WebProxy();
                if (!string.IsNullOrWhiteSpace(uri.UserName))
                {
                    proxy.Credentials = new NetworkCredential(
                                HttpUtility.UrlDecode(uri.UserName),
                                HttpUtility.UrlDecode(uri.Password));
                }
                uri.UserName = "";
                uri.Password = "";
                proxy.Address = uri.Uri;
                proxy.BypassProxyOnLocal = true;
                proxy.BypassList = settings.NoProxy.Split(",");
                httpClientHandler.UseProxy = true;
                httpClientHandler.Proxy = proxy;
            }

            httpClientHandler.ServerCertificateCustomValidationCallback =
                (HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors) => { return true; };
            return new HttpClient(httpClientHandler);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return _client.SendAsync(requestMessage);
        }
    }
}
