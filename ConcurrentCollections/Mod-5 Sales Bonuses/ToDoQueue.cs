using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mod_5_Sales_Bonuses
{
    class ToDoQueue
    {   
        //BlockingCollection can wrap any collection that implements the IProducerConsumer interface. (so it cant wrap ConcurentDictionary)
        private readonly BlockingCollection<Trade> _queue =
            new BlockingCollection<Trade>(new ConcurrentQueue<Trade>()); 
        private bool _workingDayComplete = false;
        private readonly StaffLogsForBonuses _staffLogs;

        public ToDoQueue(StaffLogsForBonuses staffResults)
        {
            _staffLogs = staffResults;
        }

        public void AddTrade(Trade transaction)
        {
            _queue.Add(transaction);
        }

        public void CompleteAdding()
        {
            _queue.CompleteAdding();
        }

        public void MonitorAndLogTrades()
        {
            while (true)
            {
                try
                {
                    //If there is no item available in the queue the Take method will simply wait until an items become available.
                    //Now if the queue is empty and there are no more items to be expected to be in the queue, but still we try a Take() operation, then Take() will throw an error. So we need to explicitly tell the BlockingCollection that no more items are expected further.
                    //In CompleteAdding() method we make that explicit call to tell the BlockingQueue that no more items are expected
                    var nextTransaction = _queue.Take();
                    _staffLogs.ProcessTrade(nextTransaction);
                    Console.WriteLine("Processing Transaction from " + nextTransaction.Person.Name);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }
        /*
         *The depreciated version of the previous method used polling (using concurrent queue)
         */ 
      
    }
}

/*
 * Extra Features of BlockingCollection
 * 1. Take() is cancellable. So if Take is taking is too much time we can pass a cancellalation token.
 * 2. TryTake() can be used, and it allows a timeout.
 * 3. Maximum collection size can be provided. In such a scenario Add method will itself block until the collection deletes.
 * 4. Add() is also cancellable.
 * 5. TryAdd() allows timeout.
*/