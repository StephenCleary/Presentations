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

namespace SimpleAsync
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

        private void DoWork()
        {
            // Represents synchronous, blocking work.
            Thread.Sleep(5000);
        }

        private async Task DoWorkAsync()
        {
            // Represents asynchronous work.
            await Task.Delay(5000);
        }

        private void SynchronousButton_Click(object sender, RoutedEventArgs e)
        {
            SynchronousLabel.Content = "Working...";
            DoWork();
            SynchronousLabel.Content = "Done!";
        }

        private async void AsychronousButton_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousLabel.Content = "Working...";
            await DoWorkAsync();
            AsynchronousLabel.Content = "Done!";
        }
    }
}
