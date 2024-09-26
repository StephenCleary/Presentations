using Nito.Logging;

internal sealed class ExceptionLoggingScopeProvider : ILoggerProvider, ISupportExternalScope
{
	private readonly ILoggerProvider _inner;

	public ExceptionLoggingScopeProvider(ILoggerProvider inner) => _inner = inner;

	ILogger ILoggerProvider.CreateLogger(string categoryName) => new Logger(_inner.CreateLogger(categoryName));

	void IDisposable.Dispose() => _inner.Dispose();

	void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
	{
		if (_inner is ISupportExternalScope supportExternalScope)
			supportExternalScope.SetScopeProvider(scopeProvider);
	}

	private sealed class Logger : ILogger
	{
		private readonly ILogger _inner;

		public Logger(ILogger inner) => _inner = inner;

		IDisposable? ILogger.BeginScope<TState>(TState state) => _inner.BeginScope(state);

		bool ILogger.IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel);

		void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			using var scopes = _inner.BeginCapturedExceptionLoggingScopes(exception);
			_inner.Log(logLevel, eventId, state, exception, formatter);
		}
	}
}