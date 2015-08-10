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

        private async Task DoWorkAsync()
        {
            ListBox.Items.Add("Hi!");
            var task1 = Task.Delay(TimeSpan.FromSeconds(2));
            var task2 = Task.Delay(TimeSpan.FromSeconds(2));
            await Task.WhenAll(task1, task2);
            ListBox.Items.Add("Still here!");
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            await DoWorkAsync();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
