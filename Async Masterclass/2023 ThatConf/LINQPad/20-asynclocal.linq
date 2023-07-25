<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	DisplayLocal();
	local.Value = 13;
	DisplayLocal();
	// await Task.Run(() => DisplayLocal());
	
	await MethodAsync();
	DisplayLocal();
}

static readonly AsyncLocal<int> local = new();

void DisplayLocal()
{
	local.Value.Dump();
	//$"{Thread.CurrentThread.ManagedThreadId}: {local.Value}".Dump();
}

async Task MethodAsync()
{
	local.Value = 7;
	await Task.Yield();
	DisplayLocal();
}