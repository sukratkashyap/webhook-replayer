using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace WebhookReplayer.Core
{
    public class Settings
    {
        public string HttpProxy { get; private set; }

        public string HttpsProxy { get; private set; }

        public string NoProxy { get; private set; }

        public Settings(IConfiguration configuration)
        {
            HttpProxy = configuration["http_proxy"];
            HttpsProxy = configuration["https_proxy"];
            NoProxy = configuration["no_proxy"];
        }
    }
}
