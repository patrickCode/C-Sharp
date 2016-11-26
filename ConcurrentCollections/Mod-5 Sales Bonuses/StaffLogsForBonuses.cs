using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;

namespace Mod_5_Sales_Bonuses
{
    class StaffLogsForBonuses
    {
        private ConcurrentDictionary<SalesPerson, int> _salesByPerson = new ConcurrentDictionary<SalesPerson, int>();
        private ConcurrentDictionary<SalesPerson, int> _purchaseByPerson = new ConcurrentDictionary<SalesPerson, int>(); 
        public void ProcessTrade(Trade sale)
        {
            Thread.Sleep(300); //Deliberatly done to increase task timer
            if (sale.QuantitySold > 0)
                _salesByPerson.AddOrUpdate(sale.Person, sale.QuantitySold,
                    (key, oldValue) => oldValue + sale.QuantitySold);
            else
                _purchaseByPerson.AddOrUpdate(sale.Person, -sale.QuantitySold,
                    (key, oldValue) => oldValue - sale.QuantitySold);
        }

        public void DisplayReport(SalesPerson[] people) 
        {
            Console.WriteLine();
            Console.WriteLine("Transaction by salesperson:");
            foreach (var person in people)
            {
                var sales = _salesByPerson.GetOrAdd(person, 0);
                var purchases = _purchaseByPerson.GetOrAdd(person, 0);
                Console.WriteLine("{0, 15} sold {1,3}, bought {2,3} items, total {3}", person.Name, sales, purchases,
                    sales + purchases);
            }
        }
    }
}
