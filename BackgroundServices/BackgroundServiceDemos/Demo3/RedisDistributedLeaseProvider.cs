// From: https://gist.github.com/StephenCleary/9777f2dcb9834a05efb261cc36839b3c

using StackExchange.Redis;
using Nito.Logging;

public interface IDistributedLeaseProvider
{
	ValueTask<IDistributedLease?> TryAcquireLeaseAsync(string leaseName, TimeSpan leaseTimeout);
}

public interface IDistributedLease : IAsyncDisposable
{
	ValueTask<bool> TryExtendLeaseAsync(TimeSpan newLeaseTimeout);
}

public sealed class RedisDistributedLeaseProvider(ILogger logger, IDatabaseAsync database, string? owner = null)
	: IDistributedLeaseProvider
{
	private readonly ILogger _logger = logger;
	private readonly IDatabaseAsync _database = database;
	private readonly string _owner = owner ?? Guid.NewGuid().ToString("N");

	public async ValueTask<IDistributedLease?> TryAcquireLeaseAsync(string leaseName, TimeSpan leaseTimeout)
	{
		var leaseKey = $"lease:{leaseName}";
		using var _ = _logger.BeginDataScope(("LeaseName", leaseName), ("LeaseOwner", _owner));
		if (await _database.StringSetAsync(leaseKey, _owner, leaseTimeout == Timeout.InfiniteTimeSpan ? null : leaseTimeout, When.NotExists))
		{
			_logger.LogInformation("Acquired lease with timeout {leaseTimeout}.", leaseTimeout);
			return new DistributedLease(this, leaseName);
		}
		else
		{
			_logger.LogInformation("Failed to acquire lease.");
			return null;
		}
	}

	private async ValueTask<bool> TryExtendLeaseAsync(string leaseName, TimeSpan leaseTimeout)
	{
		var leaseKey = $"lease:{leaseName}";
		using var _ = _logger.BeginDataScope(("LeaseName", leaseName), ("LeaseOwner", _owner));
		if (await DoExtendAsync(_database, leaseKey, _owner, leaseTimeout == Timeout.InfiniteTimeSpan ? -1 : (int)leaseTimeout.TotalMilliseconds))
		{
			_logger.LogInformation("Extended lease to {leaseTimeout}.", leaseTimeout);
			return true;
		}
		else
		{
			_logger.LogInformation("Failed to extend lease.");
			return false;
		}

		static async ValueTask<bool> DoExtendAsync(IDatabaseAsync database, RedisKey leaseKey, RedisValue owner, RedisValue leaseTimeoutMilliseconds)
		{
			const string script = """
				if redis.call("get", KEYS[1]) == ARGV[1] then
					if ARGV[2] == -1 then
						return redis.call("persist", KEYS[1])
					else
						return redis.call("pexpire", KEYS[1], ARGV[2])
				else
					return 0
				end
				""";
			var scriptResult = await database.ScriptEvaluateAsync(script, keys: [leaseKey], values: [owner, leaseTimeoutMilliseconds]);
			return (int)scriptResult == 1;
		}
	}

	private async ValueTask ReleaseLeaseAsync(string leaseName)
	{
		var leaseKey = $"lease:{leaseName}";
		using var _ = _logger.BeginDataScope(("LeaseName", leaseName), ("LeaseOwner", _owner));
		if (await DoReleaseAsync(_database, leaseKey, _owner))
			_logger.LogInformation("Released lease.");
		else
			_logger.LogInformation("Failed to release lease.");

		static async ValueTask<bool> DoReleaseAsync(IDatabaseAsync database, RedisKey leaseKey, RedisValue owner)
		{
			const string script = """
				if redis.call("get", KEYS[1]) == ARGV[1] then
					return redis.call("del", KEYS[1])
				else
					return 0
				end
				""";
			var scriptResult = await database.ScriptEvaluateAsync(script, keys: [leaseKey], values: [owner]);
			return (int)scriptResult == 1;
		}
	}

	private sealed class DistributedLease(RedisDistributedLeaseProvider provider, string name)
		: IDistributedLease
	{
		private readonly RedisDistributedLeaseProvider _provider = provider;
		private readonly string _name = name;

		public ValueTask<bool> TryExtendLeaseAsync(TimeSpan newLeaseTimeout) => _provider.TryExtendLeaseAsync(_name, newLeaseTimeout);

		public async ValueTask DisposeAsync() => await _provider.ReleaseLeaseAsync(_name);
	}
}