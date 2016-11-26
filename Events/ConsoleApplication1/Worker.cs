using System;
using System.Threading;

namespace ConsoleApplication1
{
    public class Worker
    {   
        public event WorkPerformedHandler WorkPerformedCustom;
        //EventHandler is an in-built delegate
        public event EventHandler WorkCompleted;
        //Standard Way to Handle Events in .NET
        //Event Handler delegate will be used so we don't have to create custom delegate, and the required information will be passed using 
        public event EventHandler<WorkPerformedEventArgs> WorkPerformed;
        public void Process(int hours, WorkType type)
        {
            var hoursLeft = hours;
            while (hoursLeft > 0)
            {
                OnWorkPerformed(hours - hoursLeft, type);
                Thread.Sleep(100);
                hoursLeft--;
            }
            OnWorkCompleted();
        }

        protected virtual void OnWorkPerformed(int hour, WorkType type)
        {
            //Technique 1
            //if (WorkPerformed != null)  //Check if there are listeners
            //    WorkPerformed(hour, type);

            //Technique 2
            var del = WorkPerformedCustom as WorkPerformedHandler;    //Get the delegate, since event is just a syntactic sugar on top of delegate.
            if (del != null)    //Check if there are listeners
                del(hour, type);

            WorkPerformed(this, new WorkPerformedEventArgs(hour, type));
        }

        protected virtual void OnWorkCompleted()
        {
            var del = WorkCompleted as EventHandler;
            if (del != null)
                del(this, EventArgs.Empty);
        }
    }
    /*
     * For passing custom data through event args, the EventArgs class can be extended.
     */
}