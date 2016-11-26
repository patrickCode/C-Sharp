using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_6_Lesson_2___Enumerators
{
    //If we try to enumerate a concurrent queue, stack or bag, then before enumeration can be started a snapshot of the state is taken. So it is guarnteed that even if we modify the state of the collection while enumerating, the change wont be shown in that enumeration.
    //For taking the snapshot of a ConcurrentDictinary we need to use either of the below:
    //foreach (var item in stock.ToArray())
    //foreach (var item in stock.Keys)
    //foreach (var item in stock.Values)
    //However all the above operations query the aggregate state of the dictionary hence they may be time consuming.
    class Program
    {
        static void Main(string[] args)
        {
            EnumerateDictionary();

            EnumerateConcurrentDictionary();

            Console.ReadLine();
        }

        static void EnumerateDictionary()
        {
            var stock = new Dictionary<string, int>
            {
                {"Lee", 5},
                {"GAP", 4},
                {"Turtle", 6},
                {"Amul", 1}
            };
            try
            {
                foreach (var item in stock)
                {
                    stock["Lee"] += 1;  //Cannot update dicitionary while enumerating, no standard collection can be updated while enumerating.
                    Console.WriteLine(item.Key + ": " + item.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Cannot Update value while Enumerating");
                Console.WriteLine(ex.Message);
            }
        }

        static void EnumerateConcurrentDictionary()
        {
            var stock = new ConcurrentDictionary<string, int>();
            stock.TryAdd("Lee", 4);
            stock.TryAdd("GAP", 5);
            stock.TryAdd("Turtle", 3);
            stock.TryAdd("Amul", 2);
            
            foreach (var item in stock)
            {
                //No Error will be thrown. However there is no guarantee that the enumeration will produce the corrent result.
                stock.AddOrUpdate("Lee", 0, (key, value) => value + 1);
                Console.WriteLine(item.Key + ": " + item.Value);
            }
        }
    }
}
