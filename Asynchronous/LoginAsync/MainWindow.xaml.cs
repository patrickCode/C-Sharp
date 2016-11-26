using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LoginAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            BusyIndicator.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = false;
            //Whatever delegate/action is passed to Task.Run, that is put in the thread pool and when a thread is available the action will be executed.
            var task = Task.Run(() =>
            {   
                //Dummy Login
                Thread.Sleep(2000);
                var isSuccess = false;
                Dispatcher.Invoke(() =>
                {
                    if (Username.Text.Equals("pratikb") && Password.Password.Equals("SamplePassword"))
                        isSuccess = true;
                });
                if (isSuccess)
                    return "Login Successfull";
                throw new UnauthorizedAccessException();
            });

            //ConfigureAwait allows the continuation to be executed on the calling thread. If we had ConfigureAwait to false then the continuation wouldn't have been executed on the calling thread.
            task.ConfigureAwait(true)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    if (task.IsFaulted)
                    {
                        BusyIndicator.Visibility = Visibility.Hidden;
                        LoginButton.Content = "Login Failed";
                        LoginButton.IsEnabled = true;
                    }
                    else
                    {
                        BusyIndicator.Visibility = Visibility.Hidden;
                        //task.Result will block the execution until result is avaiable on the thread. So if it's used outside the continue then the thread will get blocked.
                        LoginButton.Content = task.Result;
                        LoginButton.IsEnabled = true;
                    }
                });

            /* --- ContinueWith
            //The disadvantage with ContinueWith is that it doesn't run on the UI thread so we need to call the Dispather thread.

            //ContinueWith will be called when the task is complete
            //t is the Thread whose execution is over
            task.ContinueWith((t) =>
            {
                //If Exception was thrown while the thead was being completed.
                if (t.IsFaulted)
                {
                    Dispatcher.Invoke(() =>
                    {
                        BusyIndicator.Visibility = Visibility.Hidden;
                        LoginButton.Content = "Login Failed";
                        LoginButton.IsEnabled = true;
                    });
                }
                else
                {
                    //Dispatcher is required because we need the UI thread to do the UI modifications.
                    Dispatcher.Invoke(() =>
                    {
                        BusyIndicator.Visibility = Visibility.Hidden;
                        //t.Result will block the execution until result is avaiable on the thread. So if it's used outside the continue then the thread will get blocked.
                        LoginButton.Content = t.Result;
                        LoginButton.IsEnabled = true;
                    });
                }
            });
            */
        }
    }
}