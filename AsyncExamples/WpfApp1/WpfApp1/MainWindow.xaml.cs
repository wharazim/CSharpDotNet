using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ((System.Windows.Media.Animation.Storyboard)FindResource("WaitStoryboard")).Begin();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            var task = Task.Run(() =>
            {
                for (var i = 0; i < 10; i++)
                {
                    //throw new UnauthorizedAccessException();
                    Thread.Sleep(500);
                    Dispatcher.Invoke(() => progressBar1.Value += 10);
                }

                return "Ok";
            });

            // here we're telling the awaiter to use the main UI thread for contuniations
            task.ConfigureAwait(true)           // <-- setting this to false would tell the awaiting to execute the continuation on the owner thread
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    // so this code executes on the main UI
                    label1.Content = task.Result;
                    button1.IsEnabled = true;
                });

            // continue with not executed on main UI thread
            //task.ContinueWith((t) =>
            //{
            //    if (t.IsFaulted)
            //    {
            //        //we call dispatcher to tell the continuation to use the main UI thread
            //        Dispatcher.Invoke(() =>
            //        {
            //            button1.Content = "Bad!";
            //            button1.IsEnabled = true;
            //        });
            //    }
            //    else
            //    {
            //        Dispatcher.Invoke(() =>
            //        {
            //            button1.Content = t.Result;
            //            button1.IsEnabled = true;
            //        });
            //    }
            //});
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;
            spinner.Visibility = Visibility.Visible;
            progressBar2.Value = 0;

            try
            {
                var result = await LoginWithAdditionalAsync();
                label2.Content = result;
            }
            catch (Exception ex)
            {
                label2.Content = $"Duh!! {ex.Message}";
            }

            spinner.Visibility = Visibility.Hidden;
            button2.IsEnabled = true;
        }

        private async Task<string> LoginAsync()
        {
            //throw new UnauthorizedAccessException();
            try
            {
                var result = await Task.Run(() =>
                {
                    //throw new UnauthorizedAccessException();
                    for (var i = 0; i < 10; i++)
                    {
                        //throw new UnauthorizedAccessException();
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() => progressBar2.Value += 10);
                    }

                    return "Ok";
                });

                await Task.Delay(2000); // simulate getting additional data

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Simulate a three task async method where all tasks execute concurrently and we wait for all to finish before executing the continuation
        /// </summary>
        /// <returns></returns>
        private async Task<string> LoginWithAdditionalAsync()
        {
            //throw new UnauthorizedAccessException();
            try
            {
                var loginTask = Task.Run(() =>
                {
                    //throw new UnauthorizedAccessException();
                    for (var i = 0; i < 10; i++)
                    {
                        //throw new UnauthorizedAccessException();
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() => progressBar2.Value += 10);
                    }

                    return "Ok";
                });

                var loggingTask = Task.Delay(2000); // simulate getting additional data
                var cleanupTask = Task.Delay(1000); // another simulation

                await Task.WhenAll(loginTask, loggingTask, cleanupTask);

                return loginTask.Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
