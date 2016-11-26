using System;

namespace ConsoleApplication1
{
    public class WorkPerformedEventArgs: EventArgs
    {
        public WorkPerformedEventArgs(int hours, WorkType type)
        {
            Hours = hours;
            Type = type;
        }
        public int Hours { get; set; }
        public WorkType Type { get; set; }
    }
}