using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileConverterWebApi.Authorization
{
    public class ApiKeyAuthorizationModule
    {
        readonly RequestDelegate _next;
        const string ApiKeyHeaderName = "Authorization";

        public ApiKeyAuthorizationModule(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiKeyProvider apiKeyProvider)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Please provide ApiKey in Authorization header");
                return;
            }

            if (!extractedApiKey.Equals(apiKeyProvider.GetApiKey()))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Wrong ApiKey");
                return;
            }

            await _next(context);
        }
    }
}