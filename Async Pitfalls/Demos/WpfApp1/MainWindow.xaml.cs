using System.Windows;

namespace WpfApp1;

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

	private async void Simple_Click(object sender, RoutedEventArgs e)
	{
		await DoWorkAsync();
	}
		
	#region Cancellation and Progress

	private CancellationTokenSource? _cts;

	private async void Complex_Click(object sender, RoutedEventArgs e)
	{
		_cts = new CancellationTokenSource();
		var progress = new Progress<string>(s => ListBox.Items.Add(s));
		try
		{
			await DoStuffAsync(_cts.Token, progress);
		}
		catch (OperationCanceledException)
		{
			ListBox.Items.Add("Cancelled!");
		}
	}

	private void Cancel_Click(object sender, RoutedEventArgs e)
	{
		_cts?.Cancel();
	}

	private static async Task DoStuffAsync(CancellationToken cancellationToken, IProgress<string> progress)
	{
		for (int i = 0; i != 5; ++i)
		{
			progress?.Report($"Working: {i}");
			await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
		}

		progress?.Report("Done!");
	}

	#endregion
}