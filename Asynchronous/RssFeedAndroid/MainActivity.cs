using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Threading;

namespace RssFeedAndroid
{
    [Activity(Label = "RssFeedAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var counterButton = FindViewById<Button>(Resource.Id.CounterButton);
            var rssButton = FindViewById<Button>(Resource.Id.RssButton);

            counterButton.Click += delegate {
                var counterTextView = FindViewById<TextView>(Resource.Id.CounterText);
                counterTextView.Text = $"Counter {count++}";
            };

            rssButton.Click += delegate {
                var client = new WebClient();
                var data = client.DownloadString("http://www.filipekberg.se/rss/");

                Thread.Sleep(10000);

                var rssTextView = FindViewById<TextView>(Resource.Id.Rss);

                rssTextView.Text = data;
            };
        }
    }
}

