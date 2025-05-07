using Nito.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Demo3;

// docker run --rm -it -p 5672:5672 -p 15672:15672 rabbitmq:4-management
// http://localhost:15672

public class QueueWorker(ILogger<QueueWorker> logger,
	IHostApplicationLifetime hostApplicationLifetime,
	IServiceScopeFactory serviceScopeFactory) :
	FixedWorkingBackgroundService2Ex(logger, hostApplicationLifetime)
{
	protected override async Task DoExecuteAsync(CancellationToken stoppingToken)
	{
		await using var connection = await new ConnectionFactory().CreateConnectionAsync(stoppingToken);
		await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

		await channel.QueueDeclareAsync("demo", durable: true, autoDelete: true, cancellationToken: stoppingToken);

		using var queueLoggingScope = Logger.BeginDataScope(
			("queue", "demo"),
			("queue_server", connection.Endpoint.HostName));

		var consumer = new AsyncEventingBasicConsumer(channel);
		consumer.ReceivedAsync += async (_, args) =>
		{
			// (only necessary because RabbitMQ gives us an awkward event API instead of an async stream)
			using var queueLoggingScope = Logger.BeginDataScope(
				("queue", "demo"),
				("queue_server", connection.Endpoint.HostName));

			using var messageLoggingScope = Logger.BeginDataScope(("message_id", args.BasicProperties.MessageId));
			await using var scope = serviceScopeFactory.CreateAsyncScope();

			var message = Encoding.UTF8.GetString(args.Body.Span);
			await HandleMessage(scope.ServiceProvider, message);
		};

		var consumerTag = await channel.BasicConsumeAsync("demo", autoAck: false, consumer, stoppingToken);
		Logger.LogInformation("Listening for messages as {consumer}...", consumerTag);

		await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
	}

	private static Task HandleMessage(IServiceProvider serviceProvider, string message)
	{
		// Use the provider to resolve services that may have a per-message lifetime.
		var localLogger = serviceProvider.GetRequiredService<ILogger<QueueWorker>>();
		localLogger.LogInformation("Received {message}", message);
		return Task.CompletedTask;
	}
}
