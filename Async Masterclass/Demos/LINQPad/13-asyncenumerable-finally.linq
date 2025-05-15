<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	await foreach (var item in SomeMethod())
		item.Dump("Yielded item");
}

async IAsyncEnumerable<int> SomeMethod()
{
	try
	{
		await Task.CompletedTask;
		yield return 13;
	}
	finally
	{
		await Task.Delay(7);
		"hi!".Dump();
	}
}
