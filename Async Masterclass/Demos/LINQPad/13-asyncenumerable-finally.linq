<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
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
