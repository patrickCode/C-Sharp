using System;
using System.Threading;

namespace LocVarAsync
{
    public class ServiceProvider
    {
        private readonly ProxyService<DummyWcfService> _proxyService;
        public ServiceProvider()
        {
            _proxyService = new ProxyService<DummyWcfService>();
        }

        public string CreateData(string guid)
        {
            var createdData = string.Empty;
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            _proxyService.Call(svc =>
            {
                createdData = svc.GetData();
            });
            createdData += " "+currentThreadId + "_" + guid;
            var random = new Random();
            Thread.Sleep(random.Next(100, 20000));
            return createdData;
        }
    }
}