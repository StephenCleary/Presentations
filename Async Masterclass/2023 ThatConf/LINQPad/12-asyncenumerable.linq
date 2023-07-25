<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
}

async IAsyncEnumerable<int> SomeMethod()
{
	await Task.CompletedTask;
	yield return 13;
}
