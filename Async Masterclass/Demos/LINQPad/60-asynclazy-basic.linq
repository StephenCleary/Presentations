<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

async Task Main()
{
	var lazy = new AsyncLazy<int>(async () =>
	{
		await Task.Delay(2000);
		return 13;
	});
	
	var result = await lazy;
	result.Dump();
}

sealed class AsyncLazy<T>
{
}