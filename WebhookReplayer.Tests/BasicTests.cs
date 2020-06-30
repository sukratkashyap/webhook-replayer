using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebhookReplayer.Tests
{
    public class Tests
    {
        private HttpClient _sut;

        [SetUp]
        public void Setup()
        {
            _sut = ApplicationTesting.Client;
        }

        [Test]
        public async Task TestAnyUrl()
        {
            var response = await _sut.GetAsync("/testingggg?hello=hello");
            response.EnsureSuccessStatusCode();
        }
    }
}
