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

namespace SimpleMultithreading
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
            // Represents CPU-bound work.
            for (int i = 0; i != 300; ++i)
                Thread.Sleep(1);
        }

        private void SynchronousButton_Click(object sender, RoutedEventArgs e)
        {
            SynchronousLabel.Content = "Working...";
            DoWork();
            SynchronousLabel.Content = "Done!";
        }

        private async void MultithreadedButton_Click(object sender, RoutedEventArgs e)
        {
            MultithreadedLabel.Content = "Working...";
            Task task = Task.Run(() => DoWork());
            // For now, just ignore the async/await here.
            await task;
            MultithreadedLabel.Content = "Done!";
        }
    }
}
