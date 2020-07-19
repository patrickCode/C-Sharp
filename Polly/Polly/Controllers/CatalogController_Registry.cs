using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.Registry;
using Polly.Retry;

namespace PollySample.Controllers
{
    [ApiController]
    public class Catalog2Controller : ControllerBase
    {
        private readonly PolicyRegistry _registry;


        public Catalog2Controller(PolicyRegistry registry)
        {
            _registry = registry;
        }

        [HttpGet]
        [Route("api/products_2/{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44363/");

            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = _registry.Get<AsyncRetryPolicy<HttpResponseMessage>>("WaitAndRetry");

            var response = await retryPolicy.ExecuteAsync(() => httpClient.GetAsync($"api/inventories/{productId}"));

            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }
    }
}
