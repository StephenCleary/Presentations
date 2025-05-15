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
	await Task.CompletedTask;
	yield return 13;
}
