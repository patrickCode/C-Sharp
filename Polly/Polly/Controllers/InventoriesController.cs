using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollySample.Controllers
{
    [ApiController]
    [Route("api/inventories")]
    public class InventoriesController : ControllerBase
    {
        private static int _counter = 0;

        [HttpGet]
        [Route("{productId}")]
        public IActionResult GetInventory(string productId)
        {
            if (productId == "3")
                Task.Delay(2000).Wait();
            if (productId == "5")
            {
                Task.Delay(10000).Wait();
                return new OkObjectResult("Result");
            }
                
            _counter++;
            if (_counter % 4 == 0)
                return new OkObjectResult("Result");
            throw new Exception("Unhandled error");
        }
    }
}
