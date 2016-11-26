using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_4_Lesson_4_Concurrent_Bag
{
    class Program
    {
        /*
         * ConcurrentBag gives no guarantee of the order in which the items are deleted.
         * Internally ConcurrentBag maintains a separate collection of items for each thread that tries to acces it.
         * So when a thread tries to add or take an item, the bag will use that threads local collection.
         * Within a single thread, it works like a Stack.
         * If a thead doesnt have any item in its own local collection, but still tries to take an item, then concurrent bag will give it an inten from another thread's collection. This is where syncronization is required (stealing the item).
         * Except from the Stealing scenario (above case), no thread syncronization is required. Hence ConcurrentBag can be very fast, provided that same thread add and take items (minimize stealing).
         * ConcurrentBags are generally used when the user doesn't care what items are being taken from the collection, but requires an effective and fast method to take an item out.
         */ 
        static void Main(string[] args)
        {
            var shirts = new ConcurrentBag<string>();
            shirts.Add("Lee");
            shirts.Add("GAP");
            shirts.Add("Turtle");

            Console.WriteLine("After Push. Count = " + shirts.Count);

            string shirt1;
            if (shirts.TryTake(out shirt1))
                Console.WriteLine("\r\nRemoving - " + shirt1);
            else
                Console.WriteLine("Queue was empty");


            string shirt2;
            if (shirts.TryPeek(out shirt2))
                Console.WriteLine("Peeking - " + shirt2);
            else
                Console.WriteLine("Queue was empty");

            Console.WriteLine("\r\nEnumerating");
            foreach (var shirt in shirts)
                Console.WriteLine(shirt);

            Console.WriteLine("\r\nAfter Enumerating, count = " + shirts.Count);

            Console.ReadLine();
        }
    }
}
