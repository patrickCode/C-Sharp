using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace PollySample.Controllers
{
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _httpWaitAndRetryPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _policyWrap;
        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                    .CircuitBreakerAsync(1, TimeSpan.FromSeconds(15),
                        onBreak: (delegateResult, timeSpan, context) => { Debug.WriteLine("On Break"); },
                        onReset: (context) => { Debug.WriteLine("On Break"); },
                        onHalfOpen: () => { Debug.WriteLine("On Break"); });

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _advancedCircuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
            .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(60), 7, TimeSpan.FromSeconds(15),
            onBreak: (delegateResult, timeSpan, context) => { Debug.WriteLine("On Break"); },
                        onReset: (context) => { Debug.WriteLine("On Break"); },
                        onHalfOpen: () => { Debug.WriteLine("On Break"); });

        private static readonly AsyncBulkheadPolicy<HttpResponseMessage> _bulkHeadPolicy =
            Policy.BulkheadAsync<HttpResponseMessage>(2, 4, onBulkheadRejectedAsync: (context) => { Debug.WriteLine("Bulkhead rejected"); return Task.CompletedTask; });

        private IHttpClientFactory _httpClientFactory;

        public CatalogController(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;

            _httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(result => result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                .Or<TimeoutRejectedException>()
                .RetryAsync(3);

            _httpWaitAndRetryPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, sleepDurationProvider: retryCount =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryCount) / 2),
                    onRetry: (httpResponseMessage, time, context) =>
                    {
                        if (httpResponseMessage.Result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Debug.WriteLine("There was issue with authentication");
                        }
                        else
                        {
                            if (context.Contains("Logging") && context["Logging"].ToString() == "Enabled")
                            {
                                Debug.WriteLine(httpResponseMessage.Result.Content.ReadAsStringAsync().Result);
                            }
                        }
                    });

            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("Cached Default Result")
                });

            _timeoutPolicy = Policy.TimeoutAsync(seconds: 1);

            _policyWrap = _fallbackPolicy.WrapAsync(_httpWaitAndRetryPolicy.WrapAsync(_timeoutPolicy));
        }

        [HttpGet]
        [Route("api/products/{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44363/")
            };
            var contextDictionary = new Dictionary<string, object>
            {
                { "Logging", "Enabled" }
            };
            var catalogContext = new Context("CatalogContext", contextDictionary);

            // Without polly
            //var response = await httpClient.GetAsync($"api/inventories/{productId}");

            // With Polly (Retry)
            // var response = await _httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            // With Polly (Wait and Retry)
            // var response = await _httpWaitAndRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            // With Polly (Fallback and Retry)
            // var response = await _fallbackPolicy.ExecuteAsync(async () => 
            //     await _httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}")));

            // With Polly (Fallback, Retry and timeout) - Manual Wrap
            //var response = await
            //        _fallbackPolicy.ExecuteAsync(() =>
            //            _httpRetryPolicy.ExecuteAsync(() =>
            //                _timeoutPolicy.ExecuteAsync(async (token) => await httpClient.GetAsync($"api/inventories/{productId}", token), cancellationToken: CancellationToken.None)));

            // With Polly and Wrap
            // var response = await _policyWrap.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}", CancellationToken.None));

            // With Polly and context
            var response = await _policyWrap.ExecuteAsync((context) => httpClient.GetAsync($"api/inventories/{productId}", CancellationToken.None), catalogContext);

            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("api/products_alt/{productId}")]
        public async Task<IActionResult> GetProduct_Alt(int productId)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44363/")
            };

            // Circuit Breaker
            // var response = await _circuitBreakerPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            // Advanced circuit breaker
            var response = await _advancedCircuitBreakerPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("api/products_alt_2/{productId}")]
        public async Task<IActionResult> GetProduct_Alt_2(int productId)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44363/")
            };

            var response = await _bulkHeadPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("api/products_alt_3/{productId}")]
        public async Task<IActionResult> GetProduct_Alt_3(int productId)
        {
            var httpClient = _httpClientFactory.CreateClient("RemoteServer");
            var response = await httpClient.GetAsync($"api/inventories/{productId}"); // Polly policies are injected from Startup
            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }
    }
}
