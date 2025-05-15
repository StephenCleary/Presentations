<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

void Main()
{
}

class Response { }

sealed class TcsDictionary
{
	private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Response>> _dictionary = new();
	
	public Task<Response> Add(Guid requestId)
	{
		var tcs = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);
		_dictionary.AddOrUpdate(requestId, _ => tcs,
			(_, _) => throw new InvalidOperationException("Multiple request ids"));
		return tcs.Task;
	}
	
	public bool TrySetResult(Guid requestId, Response response)
	{
		if (!_dictionary.TryRemove(requestId, out var tcs))
			return false;
		return tcs.TrySetResult(response);
	}

	public bool TrySetException(Guid requestId, Exception exception)
	{
		if (!_dictionary.TryRemove(requestId, out var tcs))
			return false;
		return tcs.TrySetException(exception);
	}

	// Also useful: CancelAll / Dispose
}