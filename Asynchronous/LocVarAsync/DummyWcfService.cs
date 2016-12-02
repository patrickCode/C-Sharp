using System.Threading;

namespace LocVarAsync
{
    public class DummyWcfService
    {
        public string GetData()
        {
            return $"Some_Custom_Data_in_Thread {Thread.CurrentThread.ManagedThreadId}";
        }
    }
}