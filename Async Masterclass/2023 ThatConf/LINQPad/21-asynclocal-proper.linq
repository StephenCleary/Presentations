<Query Kind="Program">
  <NuGetReference>Nito.Disposables</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Nito.Disposables</Namespace>
</Query>

async Task Main()
{
	DisplayLocal();

	using (MyLocal.Push(13))
	{
		DisplayLocal();
		// await Task.Run(() => DisplayLocal());

		await MethodAsync();
		DisplayLocal();
	}
}

void DisplayLocal()
{
	MyLocal.Current.Dump();
	//$"{Thread.CurrentThread.ManagedThreadId}: {MyLocal.Current}".Dump();
}

async Task MethodAsync()
{
	using (MyLocal.Push(7))
	{
		await Task.Yield();
		DisplayLocal();
	}
}

static class MyLocal
{
	public static int Current => _local.Value;
	
	public static IDisposable Push(int value)
	{
		var oldValue = _local.Value;
		_local.Value = value;
		return Disposable.Create(() => _local.Value = oldValue);
	}
	
	private static readonly AsyncLocal<int> _local = new();
}