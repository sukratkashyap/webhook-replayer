using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebhookReplayer.Tests
{
    public static class Extensions
    {
        public static async Task<T> ToJson<T>(this HttpContent content)
        {
            var body = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}
