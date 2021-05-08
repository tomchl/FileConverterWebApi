using System;
using System.Net;
using System.Threading.Tasks;
using FileConverterWebApi.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace FileConverterWebApiTest
{
    [TestFixture]
    public class ApiKeyAuthorizationModuleTest
    {
        ApiKeyAuthorizationModule _module;
        Mock<IApiKeyProvider> _apiKeyMock;

        [SetUp]
        public void SetUp()
        {
            _apiKeyMock = new Mock<IApiKeyProvider>();
            _apiKeyMock.Setup(x => x.GetApiKey()).Returns("XXX");
            _module = new ApiKeyAuthorizationModule(hc => Task.CompletedTask);
        }

        [Test]
        public void Request_DoesNotContainApiKeyHeader()
        {
            HttpContext ctx = new DefaultHttpContext();
            _module.InvokeAsync(ctx, _apiKeyMock.Object);

            ctx.Response.StatusCode.Should().Be(401);
        }

        [Test]
        public void Request_DoContainWrongApiKeyHeader()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Headers.Add("Authorization", "XX");
            _module.InvokeAsync(ctx, _apiKeyMock.Object);

            ctx.Response.StatusCode.Should().Be(401);
        }

        [Test]
        public void Request_OK()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Headers.Add("Authorization", "XXX");
            _module.InvokeAsync(ctx, _apiKeyMock.Object);

            ctx.Response.StatusCode.Should().NotBe(401);
        }
    }
}