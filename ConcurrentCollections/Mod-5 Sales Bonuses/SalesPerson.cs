using System;
using System.Threading;

namespace Mod_5_Sales_Bonuses
{
    class SalesPerson
    {
        public string Name { get; private set; }

        public SalesPerson(string id)
        {
            this.Name = id;
        }

        public void Work(StockController stockController, TimeSpan workDay)
        {
            var rand = new Random(Name.GetHashCode());
            var start = DateTime.Now;
            while (DateTime.Now - start < workDay)
            {
                Thread.Sleep(rand.Next(100)); //Comment this to stress test the item, if the sales person don't sleep then a lot of transactions will occur
                var buy = (rand.Next(6) == 0);
                var itemName = Program.AllShirtNames[rand.Next(Program.AllShirtNames.Count)];
                if (buy)
                {
                    var quantity = rand.Next(9) + 1;
                    //Item can be bought anu time
                    stockController.BuyStock(this, itemName, quantity);
                    DisplayPurchase(itemName, quantity);
                }
                else
                {
                    //Items can only be sold if the stock exist
                    var success = stockController.TrySellItem(this, itemName);
                    DisplaySaleAttempt(success, itemName);
                }
            }
            Console.WriteLine("Sales Person {0} signing off", Name);
        }

        private void DisplayPurchase(string itemName, int quantity)
        {
            Console.WriteLine("Thread {0}: {1} bought {2} of {3}", Thread.CurrentThread.ManagedThreadId, this.Name,
                itemName, quantity);
        }

        private void DisplaySaleAttempt(bool success, string itemName)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(success
                ? string.Format("Thread {0}: {1} sold {2}", threadId, this.Name, itemName)
                : string.Format("Thread {0}: {1}: Out of Stock of {2}", threadId, this.Name, itemName));
        }
    }
}
