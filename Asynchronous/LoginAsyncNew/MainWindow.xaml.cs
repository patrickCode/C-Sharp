using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LoginAsyncNew
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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Deadlock_Demo();
            //Deadlock_Demo2();
            Deadlock_Prevention();
            try
            {
                //If we don't await for the async method to be over then we wont be able to catch the exception
                //After each await the controls goes back to the caling thread.
                await LoginAsync(Username.Text, Password.Password);
                LoginButton.Content = "Login Success";
            }
            catch (UnauthorizedAccessException)
            {
                LoginButton.Content = "Login Failed";
            }
            catch (Exception)
            {
                LoginButton.Content = "Login Error";
            }
            finally
            {
                BusyIndicator.Visibility = Visibility.Hidden;
                LoginButton.IsEnabled = true;
            }
        }


        //When we mark a function as async, the compiler generates a state machine. The State Machine is a way to keep track of the async operations and it's continuations. When the state machines catches an exception, it needs to set the exception to the Task. So if an async method return void, then it can't set the exception anywhere.
        private async Task LoginAsync(string userName, string pwd)
        {
            BusyIndicator.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = false;
            //Until the task is complete the calling context will wait it and the continuation will be executed in the calling thread itself.
            var loginTask = Task.Run(() =>
            {
                //Dummy Login
                Thread.Sleep(1000);
                if (userName.Equals("pratikb") && pwd.Equals("SamplePassword"))
                {
                    return "Login Successfull";
                }
                throw new UnauthorizedAccessException();
            });
            
            var logTask = Task.Delay(1000);
            var userDetailsTask = Task.Delay(1000);

            //Await all the tasks
            await Task.WhenAll(loginTask, logTask, userDetailsTask);
            //return loginTask.Result;
        }

        //Deadlock will happen because task.Wait will block the current thread (UI thread) until the task is complete. The task will only complete when the Dispatcher (UI Thread) is invoked. Since the UI thread is blocked by task.Wait the task cannot complete hence a deadlock arises.
        private void Deadlock_Demo()
        {
            var task = Task.Delay(1)
                .ContinueWith((t) =>
                {
                    Dispatcher.Invoke(() => { });
                });

            //Block the thread until the task is complete
            task.Wait();
        } 

        //Dealdlock Demo 2
        //Since DoWork is an async method the compiler will attach a state machine, and the state machine runs on the caller thread. .Result will block the current thread until a result is available, so there is no way for the state machine to report back becasue the UI thread is blocked.
        private void Deadlock_Demo2()
        {
            var result = DoWork().Result;
        }

        //A new thread will be spawned which will run DoWork. So the state machine will run on the new thread and not on the UI thread (which is getting blocked because of Result).
        //So we are blocking the calling thread until the result is available but not causing a deadlock.
        private void Deadlock_Prevention()
        {
            var result = Task.Run(() => DoWork()).Result;
        }

        private async Task<string> DoWork()
        {
            await Task.Delay(1000);
            return "Complete";
        }
    }
}

//Marking a method as async void is a bad practice, and it should be done only for event handlers.
//Marking a method async void will mean that we wont be able to capture error from it.