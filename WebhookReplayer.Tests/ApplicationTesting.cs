using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using WebhookReplayer.Middleware;

namespace WebhookReplayer.Tests
{
    [SetUpFixture]
    public static class ApplicationTesting
    {
        public static TestWebApplicationFactory Factory;
        public static HttpClient Client => Factory.CreateDefaultClient();


        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            Factory = new TestWebApplicationFactory();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Client?.Dispose();
            Factory?.Dispose();
        }
    }
}
