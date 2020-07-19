using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace PollySample.Controllers
{
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _httpWaitAndRetryPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;

        public CatalogController() 
        {
            _httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(result => result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                .Or<TimeoutRejectedException>()
                .RetryAsync(2);
            
            _httpWaitAndRetryPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, sleepDurationProvider: retryCount =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryCount) / 2),
                    onRetry: (httpResponseMessage, time) =>
                    {
                        if (httpResponseMessage.Result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Debug.WriteLine("There was issue with authentication");
                        }
                        else
                            Debug.WriteLine(httpResponseMessage.Result.Content.ReadAsStringAsync().Result);
                    });

            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("Cached Default Result")
                });

            _timeoutPolicy = Policy.TimeoutAsync(seconds: 1);
        }

        [HttpGet]
        [Route("api/products/{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44363/");

            // Without polly
            //var response = await httpClient.GetAsync($"api/inventories/{productId}");

            // With Polly (Retry)
            // var response = await _httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            // With Polly (Wait and Retry)
            // var response = await _httpWaitAndRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            // With Polly (Fallback and Retry)
            // var response = await _fallbackPolicy.ExecuteAsync(async () => 
            //     await _httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}")));

            // With Polly (Fallback, Retry and timeout)
            var response = await
                    _fallbackPolicy.ExecuteAsync(() =>
                        _httpRetryPolicy.ExecuteAsync(() =>
                            _timeoutPolicy.ExecuteAsync(async (token) => await httpClient.GetAsync($"api/inventories/{productId}", token), cancellationToken: CancellationToken.None)));

            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }
    }
}
