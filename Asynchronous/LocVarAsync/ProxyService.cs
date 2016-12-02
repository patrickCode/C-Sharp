using System;
using System.Threading;

namespace LocVarAsync
{
    public delegate void CallerDelegate<TProxy>(TProxy proxy);
    public class ProxyService<TProxy> where TProxy: DummyWcfService
    {
        public void Call(CallerDelegate<TProxy> method)
        {
            var random = new Random();
            Thread.Sleep(random.Next(1000, 3000));
            var service = new DummyWcfService();
            method((TProxy)service);
        }
    }
}