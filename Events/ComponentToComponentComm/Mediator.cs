using CommunicatingBetweenControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToComponentComm
{
    //The mediator is needed because the components in the screen are loaded dynamically so mediator will notify all the dynamic components about events and changes
    public sealed class Mediator
    {
        private static readonly Mediator _instance = new Mediator();
        private Mediator() { }
        //Singleton
        public static Mediator Instance
        {
            get
            {
                return _instance;
            }
        }

        public event EventHandler<JobChangedEventArgs> JobChanged;

        public void OnJobChanged(object sender, Job job)
        {
            JobChanged(sender, new JobChangedEventArgs { Job = job });
        }
    }
}
