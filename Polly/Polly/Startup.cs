using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace PollySample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(GetRegistry());
            services.AddControllers();
        }

        private PolicyRegistry GetRegistry()
        {
            var registry = new PolicyRegistry();

            AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt));

            AsyncRetryPolicy timeoutException = Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(2, attempt => TimeSpan.FromSeconds(attempt));

            registry.Add("WaitAndRetry", httpRetryPolicy);
            registry.Add("Timeout", timeoutException);

            return registry;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
