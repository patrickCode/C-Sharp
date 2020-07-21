using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

            IAsyncPolicy<HttpResponseMessage> httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(3);

            IAsyncPolicy<HttpResponseMessage> noPolicy =
                Policy.NoOpAsync<HttpResponseMessage>();

            // Polly Single Policy
            //services.AddHttpClient("RemoteServer", client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:44363/");
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
            //}).AddPolicyHandler(httpRetryPolicy);

            IPolicyRegistry<string> registry = services.AddPolicyRegistry();
            registry.Add("SimpleRetry", httpRetryPolicy);
            registry.Add("NoOp", noPolicy);

            services.AddHttpClient("RemoteServer", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44363/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandlerFromRegistry(PolicySelector);

            services.AddControllers();
        }

        private IAsyncPolicy<HttpResponseMessage> PolicySelector(IReadOnlyPolicyRegistry<string> policyRegistry, HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get)
                return policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("SimpleRetry");
            return policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("NoOp");
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
