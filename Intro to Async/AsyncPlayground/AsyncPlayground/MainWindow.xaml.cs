using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //
        // Demo 1: Thinking about tasks.
        //

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            // What operations do these tasks represent?
            // One has a result value; the other does not.

            Task delayTask = Task.Delay(TimeSpan.FromSeconds(2));

            var httpClient = new HttpClient();
            Task<string> downloadTask = httpClient.GetStringAsync("http://www.example.com");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
        }


        //
        // Demo 2: Basic async/await.
        //

        //private async Task DoWorkAsync()
        //{
        //    ListBox.Items.Add("Hi!");
        //    await Task.Delay(2000);
        //    ListBox.Items.Add("Still here!");
        //}

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    await DoWorkAsync();
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //}
        //private void Button3_Click(object sender, RoutedEventArgs e)
        //{
        //}


        //
        // Demo 3: Exceptions with Tasks and await.
        //

        //private async Task DoWorkAsync()
        //{
        //    ListBox.Items.Add("Hi!");
        //    await Task.Delay(2000);
        //    throw new Exception("Blah");
        //}

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        await DoWorkAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ListBox.Items.Add(ex.Message);
        //    }
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //}
        //private void Button3_Click(object sender, RoutedEventArgs e)
        //{
        //}


        //
        // Demo 4: Exceptions are thrown by await, not by calling the method.
        //

        //private async Task DoWorkAsync()
        //{
        //    ListBox.Items.Add("Hi!");
        //    await Task.Delay(2000);
        //    throw new Exception("Blah");
        //}

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    // No exception thrown here.
        //    var task = DoWorkAsync();
        //    try
        //    {
        //        // Exception thrown here.
        //        await task;
        //    }
        //    catch (Exception ex)
        //    {
        //        ListBox.Items.Add(ex.Message);
        //    }
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //}
        //private void Button3_Click(object sender, RoutedEventArgs e)
        //{
        //}


        //
        // Demo 5: Cancellation
        //

        //private async Task DoWorkAsync(CancellationToken token)
        //{
        //    ListBox.Items.Add("Hi!");
        //    await Task.Delay(2000, token);
        //    ListBox.Items.Add("Finished!");
        //}

        //private CancellationTokenSource _cts;

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    _cts = new CancellationTokenSource();
        //    try
        //    {
        //        await DoWorkAsync(_cts.Token);
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        ListBox.Items.Add("Canceled: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        ListBox.Items.Add("Faulted: " + ex.Message);
        //    }
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //    _cts.Cancel();
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //}


        //
        // Demo 6: Asynchronous concurrency
        //

        //private async Task DoWorkAsync()
        //{
        //    ListBox.Items.Add("Hi!");

        //    // The total time here is only 2 seconds; both delays are concurrent.
        //    var task1 = Task.Delay(2000);
        //    var task2 = Task.Delay(2000);
        //    await Task.WhenAll(task1, task2);
            
        //    ListBox.Items.Add("Done!");
        //}

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    await DoWorkAsync();
        //}

        //private void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //}
        //private void Button3_Click(object sender, RoutedEventArgs e)
        //{
        //}
    }
}
