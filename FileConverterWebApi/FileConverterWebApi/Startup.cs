using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileConverterWebApi.Authorization;
using FileConverterWebApi.Utils;

namespace FileConverterWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApiKeyProvider, ApiKeyProvider>();
            services.AddSingleton<Utils.ILogger, Logger>();
            services.AddSingleton<IFileConverter, FileConverter>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<ApiKeyAuthorizationModule>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}