using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_5_Sales_Bonuses
{
    class Program
    {
        public static readonly List<string> AllShirtNames = new List<string> { "GAP", "Max", "Lee", "Amul" };
        static void Main(string[] args)
        {
            StaffLogsForBonuses staffLogs = new StaffLogsForBonuses();
            ToDoQueue toDoQueue = new ToDoQueue(staffLogs);

            SalesPerson[] people =
            {
                new SalesPerson("Pratik"),
                new SalesPerson("Varun"),
                new SalesPerson("Pradeep"),
                new SalesPerson("Hianshu")
            };

            StockController controller = new StockController(toDoQueue);

            TimeSpan workDay = new TimeSpan(0, 0, 2);

            Task t1 = Task.Run(() => people[0].Work(controller, workDay));
            Task t2 = Task.Run(() => people[1].Work(controller, workDay));
            Task t3 = Task.Run(() => people[2].Work(controller, workDay));
            Task t4 = Task.Run(() => people[3].Work(controller, workDay));


            Task bonusLogger = Task.Run(() => toDoQueue.MonitorAndLogTrades());
            Task bonusLogger2 = Task.Run(() => toDoQueue.MonitorAndLogTrades());

            Task.WaitAll(t1, t2, t3, t4);
            toDoQueue.CompleteAdding();
            Task.WaitAll(bonusLogger, bonusLogger2);

            controller.DisplayStatus();
            staffLogs.DisplayReport(people);
            Console.ReadLine();
        }
    }
}
