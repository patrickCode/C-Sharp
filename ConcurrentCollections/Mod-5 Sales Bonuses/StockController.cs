using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Mod_5_Sales_Bonuses
{
    internal class StockController
    {
        private ConcurrentDictionary<string, int> _stock = new ConcurrentDictionary<string, int>();
        private int _totalQuantityBought;
        private int _totalQuantitySold;
        private ToDoQueue _toDoQueue;

        public StockController(ToDoQueue bonusCalculator)
        {
            _toDoQueue = bonusCalculator;
        }

        //This method is thread-safe but since it doesnt use any locks it is not atomic. Inbetween the 2 method calls, another thread can sneak in to change the value. However due to the logic of the application, that doesn't matter. As long as the _totalQuantity is getting uodated there isnt any need to worry about race condition.
        public void BuyStock(SalesPerson person, string item, int quantity)
        {
            //TryUpdate cannot be used becasue the currentValue (oldValue) is not known
            _stock.AddOrUpdate(item, quantity, (key, oldValue) => oldValue + quantity);
            Interlocked.Add(ref _totalQuantityBought, quantity);    //Thread-Safe
            _toDoQueue.AddTrade(new Trade(person, -quantity));

        }

        public bool TrySellItem(SalesPerson person, string item)
        {
            var success = false;
            int newStockLevel = _stock.AddOrUpdate(item, (itemName) =>
            {
                success = false;
                return 0;
            }, (itemName, oldValue) =>
            {
                if (oldValue == 0)
                {
                    success = false;
                    return 0;
                }
                else
                {
                    success = true;
                    return oldValue - 1;
                }
            });
            if (success)
            {
                Interlocked.Increment(ref _totalQuantitySold);
                _toDoQueue.AddTrade(new Trade(person, 1));
            }
            return success;
        }
        /*We need to return the success value, we cannot use 2 method calls, because of the multithreading problem
         * if (_stock[item]==0) return false;
         * int newLevel = _stock.AddOrUpdate(item, 0, (key, oldValue) => oldValue
         * In the above technique, there is no way to check if AddOrUpdate was able to sell the t-shirt, so in the delegate we need to set the success parameter.
         * We also need to use a delegate for the 2nd paramter, where we have to exclusively set the success paramter to false. The reason that the 3rd delegate may be executed multple times (AddOrUpdate works that way, see Mod-2 Lesson 3 for explanation) so the final value of the success paramter may ot maynot be false. Ex. AddOrUpdate runs where the success paramter is made true, before it could modify the dictioanry it sees that it is not atomic, so it will try to add/update again but this time it finds that someone has deleted the entry so it has to add, hence the 2nd delgate is called. Hence we are not actually selling the item. Now the success parameter was already changed to true in the previous iteration, unless we exclusively change it to false; once the method is over we will get a wrong idea about the state. This is called side-effect.
        */
        //We have converted the 2nd paramter to a lamda expression because the success paramter is getting changed in the 3rd delegate. As mentioned earlier that the 3rd delegate can get excuted multiple times, so the value of success may be toggled multiple times. This is called side effect. So the 2nd delegate makes sure that the success paramter is defaulted to false when it is unable to update.

        //Same Version of above method without side effects in delegate (May be Buggy)
        public bool TrySellItem(string item, string dummy)
        {
            int newStockLevel = _stock.AddOrUpdate(item, -1, (key, oldValue) => oldValue - 1);
            if (newStockLevel < 0)
            {
                _stock.AddOrUpdate(item, 1, (key, oldValue) => oldValue + 1);
                return false;
            }
            else
            {
                Interlocked.Increment(ref _totalQuantitySold);
                return true;
            }
        }
        /*Here we have broken down the same operation into 2 simpler operations. If the item is not present in the dictionary then we are 
         * We are not checking if the item was present ot not, we are simple reducing the quantity. So if the there was no stock available the new stock will be be -1. If the new stock value is -ve then we can be sure that we have sold an item which doesn't exist. Hence we put that item back in the stock.
         */

        public void DisplayStatus()
        {
            var totalStock = _stock.Values.Sum();   //Performance Hits because all threads need to be blocked
            Console.WriteLine("\r\nBought = " + _totalQuantityBought);
            Console.WriteLine("Total Quantity Sold = " + _totalQuantitySold);
            Console.WriteLine("Stock = " + totalStock);
            var error = totalStock + _totalQuantitySold - _totalQuantityBought;
            if (error == 0)
                Console.WriteLine("Stock Level Match");
            else
                Console.WriteLine("Error in Stock Level: " + error);

            Console.WriteLine();
            Console.WriteLine("Stock levels by item");
            foreach (var itemName in Program.AllShirtNames)
            {
                var stockLevel = _stock.GetOrAdd(itemName, 0);
                Console.WriteLine("{0,-30}:{1}", itemName, stockLevel);
            }
        }

    }
}
 