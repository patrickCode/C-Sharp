using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            var orders = new Queue<string>();
            //We cannot make this code multithreaded beacause Queue is not thread safe.
            PlaceOrders("Pratik B", orders);
            PlaceOrders("Pradeep B", orders);

            foreach (var order in orders)
                Console.WriteLine("ORDER: " + order);

            Console.ReadKey();
        }

        static void PlaceOrders(string customerName, Queue<string> orders)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                var orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                //Enqueue method internally has a lot of code to execute, so if multiple threads call enquue and the thread gets suspended before completing enqueue() and another thread starts manipulating the queue, thta will corrupt the queue.
                //Enqueue is not an atomic method. Atomic - 1. Other threads see the method either as complete or not started. It wont see the method as partially completed. 2. Operation will either complete succefully or fail without modifying the data.
                orders.Enqueue(orderName);
            }
        }
    }
}
