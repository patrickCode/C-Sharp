using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_2_Lesson_3_Concurrent_Dictionary__Good_Practice_
{
    /*TryGetValue()
     * If the value doenst exist in the dictionary then Try...Get doesnt throw any exception.
     * TryGetValue returns a boolean indicating whether the item was in the dictionary, and the value is returned using a out parameter.
     * All Try methods work this way, returns back a boolean and the computed value is retrieved using an out parameter.
     * TryUpdate required 2 parameters, the new value that has to be updated and also the old value. Without the correct old value(current value) TryUpdate will not update the key. So for TryUpdate to succeed, the key must be in the dictionary and the correct current value must also be supplied.
     * Try methods are atomic so even if they fail they wont make any changes to the state of the dictionary
     * AddOrUpdate takes 3 paramters. 3rd parameter is a delegate which takes the key and the oldValue as parameter and returns the new value. Internally it repeatedly evaluates the delegate and calls TryUpdate until it succeeds. A disadvantage would be that the delegate might be excecuted more than once. AddOrUpdate must always succeed, i.e. if the key wasnt present in the dictionary then the key will be created. If the key needs to be created in the dictionary then the 2nd parameter defines the initial value of the key. This method returns the value that has been added/updated to the key. The delegate that has been used to update the value shoudl not throw an exception.
     * In case of multithreaded programing it is not a good practice to retrieve value using indexer. Values should be retrieved by using return statements/out parameters from Try methods. Its more efficient and avoids race condition
     * A good principle is to DO each operation in one concurrent collection method call, becasue if multiple method calls are used then it is possible that some other thread may come and change the value in between the method call. Concurent collections do not protect user from race conditions between method calls.
     * GetOrAdd update (similar to AddOrUpdate) will never fail. This method first looks for the key in the dictionary, if its not present then add or if present then return the value. The 2nd parameter of the method specifes the value to be added for the dictionary if it was not present.
     * Tryxxx methods will either pass or fail but will never throw any exception. AddOrUpdate, GetOrAdd method will always succeed.
     */
    class Program
    {
        static void Main(string[] args)
        {
            var stock = new ConcurrentDictionary<string, int>();
            stock.TryAdd("ECO t-shirts", 4);
            stock.TryAdd("ISRM t-shirts", 5);

            Console.WriteLine(string.Format("No. of T-Shirts in stock = {0}", stock.Count));

            var success = stock.TryAdd("MPSIT t-shirts", 6);
            Console.WriteLine("Add succedded? " + success);
            //Adding will fail becasue the key already existed
            success = stock.TryAdd("MPSIT t-shirts", 6);
            Console.WriteLine("Add succedded? " + success);
            stock["ESSIT t-shirts"] = 7;

            //Using indexer to update the value in ConcurrentDictionary is a bad practice becasue in multi-threaded programming, some other thread may have updated the value. So we might overridde the changes made by some other thread. Indexer is not atomic and might introduce race condition
            //stock["MPSIT t-shirts"] = 8; //Update if exists
            success = stock.TryUpdate("MPSIT t-shirts", 7, 6);
            if (success)
                Console.WriteLine(string.Format("\r\nUpdated stock[MPSIT t-shirts] = {0}", stock["MPSIT t-shirts"]));

            //If we wanted to increment a value, the  TryUpdate may not work. In TryUpdate we have to provide the current value, the problem with the below code will be that after getting the temp value if the current thread is suspended and another thread comes and updates the value then the current thread will fail to update the value;
            //int temp;
            //stock.TryGetValue("MPSIT t-shirts", out temp);
            //stock.TryUpdate("MPSIT t-shirts", temp + 1, temp);
            //We can solve this problem by using AddOrUpdate.
            int mpsitStocks = stock.AddOrUpdate("MPSIT t-shirts", 1, (key, oldValue) => oldValue + 1);
            Console.WriteLine("Updated value: {0}", mpsitStocks);

            Console.WriteLine(string.Format("stock[MPSIT t-shirts] {0}", stock.GetOrAdd("MPSIT t-shirts", 0)));

            int isrmTShirts;
            success = stock.TryRemove("ISRM t-shirts", out isrmTShirts);
            if (success)
                Console.WriteLine("Value removed: {0}", isrmTShirts);

            Console.WriteLine("\r\nEnumerating");
            foreach (var keyValPair in stock)
            {
                Console.WriteLine("{0} : {1}", keyValPair.Key, keyValPair.Value);
            }
            Console.ReadLine();
        }
    }
}
