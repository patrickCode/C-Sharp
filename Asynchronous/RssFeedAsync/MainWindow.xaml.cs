using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RssFeedAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RssButton_Click(object sender, RoutedEventArgs e)
        {
            BusyIndicator.Visibility = Visibility.Visible;
            RssButton.IsEnabled = false;
            var client = new WebClient();
            //var data = client.DownloadString("http://www.filipekberg.se/rss/");
            client.DownloadStringAsync(new Uri("http://www.filipekberg.se/rss/"));

            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            //To simulate the screen freeze
            //Thread.Sleep(10000);
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            BusyIndicator.Visibility = Visibility.Hidden;
            RssButton.IsEnabled = true;
            RssText.Text = e.Result;
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            CounterText.Text = $"Counter: {count++}";
        }
    }
}
