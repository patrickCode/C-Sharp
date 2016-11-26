using System;
using CommunicatingBetweenControls.Model;

namespace ComponentToComponentComm
{
    public class JobChangedEventArgs: EventArgs
    {
        public Job Job { get; set; }
    }
}