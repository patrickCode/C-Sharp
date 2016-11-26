using ComponentToComponentComm;
using System.Windows.Controls;

namespace CommunicatingBetweenControls.UserControls
{
    /// <summary>
    /// Interaction logic for JobDetails.xaml
    /// </summary>
    public partial class JobDetails : UserControl
    {
        public JobDetails()
        {
            InitializeComponent();
            Mediator.Instance.JobChanged += (s, e) => DataContext = e.Job;
        }
    }
}