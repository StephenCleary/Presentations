<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
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

	public Task WaitUntilSetAsync()
	{
	}

	public void Set()
	{
	}
	
	public void Reset()
	{
	}
}

sealed class PauseTokenSource
{
	private readonly AsyncManualResetEvent _event = new();
	public void Pause() => _event.Reset();
	public void Unpause() => _event.Set();

	public struct PauseToken
	{
		private readonly PauseTokenSource _source;
		private PauseToken(PauseTokenSource source) => _source = source;
		public Task WaitWhilePausedAsync() => _source._event.WaitUntilSetAsync();
		public TaskAwaiter GetAwaiter() => WaitWhilePausedAsync().GetAwaiter();
	}
}
