<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

void Main()
{
}

sealed class AsyncManualResetEvent
{
	private TaskCompletionSource _tcs;
	
	public AsyncManualResetEvent()
	{
	}
	
	public void Set()
	{
	}
	
	public void Reset()
	{
	}
	
	public Task WaitUntilSetAsync()
	{
	}
}