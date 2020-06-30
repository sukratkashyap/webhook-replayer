using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
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
        public async Task TestingJsonPlaceHolderHttps()
        {
            var response = await _sut.GetAsync("/todos/1?_to=https://jsonplaceholder.typicode.com/");
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Test]
        public async Task TestingJsonPlaceHolderHttp()
        {
            var response = await _sut.GetAsync("/todos/1?_to=http://jsonplaceholder.typicode.com/");
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Test]
        public async Task TestingWithNoValue()
        {
            var response = await _sut.GetAsync("/test/path");
            response.StatusCode.Should().Be(500);
            var body = await response.Content.ToJson<JObject>();
            body.Value<string>("Type").Should().Be("Exception");
            body.Value<string>("Message").Should().Be("Query parameter _to=URL is missing!");
            body.Value<string>("StackTrace").Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task TestingWithEmptyValue()
        {
            var response = await _sut.GetAsync("/test/path?_to=&hello=test");
            response.StatusCode.Should().Be(500);
            var body = await response.Content.ToJson<JObject>();
            body.Value<string>("Type").Should().Be("Exception");
            body.Value<string>("Message").Should().Be("Query parameter _to is null or empty!");
            body.Value<string>("StackTrace").Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task UriParsingError()
        {
            var response = await _sut.GetAsync("/test/path?_to=gogle.cm&go=go");
            response.StatusCode.Should().Be(500);
            var body = await response.Content.ToJson<JObject>();
            body.Value<string>("Type").Should().Be("UriFormatException");
            body.Value<string>("Message").Should().Be("Invalid URI: gogle.cm/test/path?go=go");
            body.Value<string>("StackTrace").Should().NotBeNullOrWhiteSpace();
        }
    }
}
