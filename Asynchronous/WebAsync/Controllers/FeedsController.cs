using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAsync.Controllers
{
    public class FeedsController : ApiController
    {
        [HttpGet]
        public async Task<string> Get()
        {
            //In ASP.NET it's a good practice to use Configure Await because .NET will pick up a thread from the thread pool and let the continuation be carried in that. The application becomes faster
            using (var client = new HttpClient())
            {
                var httpMsg = await client.GetAsync(new Uri("http://www.filipekberg.se/rss/"))
                    .ConfigureAwait(false);
                var content = await httpMsg.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);
                return content;
            };
        }
    }
}
