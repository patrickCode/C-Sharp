using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    /* Problem with Locks
     * 1. Locking neeeds to be applied everywhere where shared state is vulnerable. (For a big application it's very difficult to lock every shared state.)
     * 2. Deadlocks can appear, if the order is not followed correctly.
     * 3. Not scalable
     * 
     */
    class WithLocks
    {
        public static void main()
        {
            var orders = new Queue<string>();
            //We cannot make this code multithreaded beacause Queue is not thread safe.
            Task task1 = Task.Run(() => PlaceOrders("Pratik B", orders));
            Task task2 = Task.Run(() => PlaceOrders("Pradeep B", orders));
            Task.WaitAll(task1, task2);

            foreach (var order in orders)
                Console.WriteLine("ORDER: " + order);

            Console.ReadKey();
        }

        static object _lockObj = new Object();
        static void PlaceOrders(string customerName, Queue<string> orders)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                var orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                lock (_lockObj)
                {
                    orders.Enqueue(orderName);
                }
            }
        }
    }
}
