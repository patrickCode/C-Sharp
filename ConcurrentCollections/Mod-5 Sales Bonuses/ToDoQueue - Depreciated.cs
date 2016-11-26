using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mod_5_Sales_Bonuses
{
    class ToDoQueueDepreciated
    {
        private readonly ConcurrentQueue<Trade> _queue = new ConcurrentQueue<Trade>();
        private readonly BlockingCollection<Trade> _blockingQueue =
            new BlockingCollection<Trade>(new ConcurrentQueue<Trade>()); 
        private bool _workingDayComplete = false;
        private readonly StaffLogsForBonuses _staffLogs;

        public ToDoQueueDepreciated(StaffLogsForBonuses staffResults)
        {
            _staffLogs = staffResults;
        }

        public void AddTrade(Trade transaction)
        {
            _queue.Enqueue(transaction);
        }

        public void CompleteAdding()
        {
            _workingDayComplete = true;
        }

        public void MonitorAndLogTrades()
        {
            while (true)
            {
                Trade nextTrade;
                bool done = _queue.TryDequeue(out nextTrade);
                if (done)
                {
                    _staffLogs.ProcessTrade(nextTrade);
                    Console.WriteLine("Processing Transaction from " + nextTrade.Person.Name);
                }
                else if (_workingDayComplete)
                {
                    //When trading day is complete no more transactions will be done so no logs are expected
                    Console.WriteLine("No more sales to log - exiting");
                    return;
                }
                else
                {
                    //There are no items in the queue, but the working day is not over yet. So the thread will sleep for some time and the check the queue again
                    Console.WriteLine("No transactions available");
                    Thread.Sleep(500);
                }
            }
        }
        /*The problem with this method is that it keeps polling the queue to check when data is available.
         * The best way to handle the polling would have been if the thread would have slept until there is some data in the queue for it to process. That can be done by using BlockingCollection.
         * BlockingCollection wraps other collections (like Queue) and provide additional services. One such service is that it can block a queue until there is some item in the queue.
         * 
         */ 
      
    }
}
