namespace TelDemo.Backend.Services;

public sealed class RabbitMqConsumerWorker(ILogger<RabbitMqConsumerWorker> logger, ReportGeneratorService reportGenerator) : BackgroundService
{
    private static readonly ActivitySource ConsumerActivitySource = new("TelDemo.Backend.Consumer");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hostName = "localhost";
        var queueName = "teldemo";

        var factory = new ConnectionFactory();
        await using var connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            using var loggingContext = logger.BeginScope(new Dictionary<string, object?>
            {
                ["RabbitMqMessageId"] = eventArgs.BasicProperties.MessageId,
                ["RabbitMqDeliveryTag"] = eventArgs.DeliveryTag,
            });

            var messageContext = Propagator.Extract(default, eventArgs.BasicProperties, ExtractTraceContextFromBasicProperties);
            Baggage.Current = messageContext.Baggage;

            // The following code creates the Consumer activity as a child of the Producer activity.
            // This is fine if your backend consumers are logically children of the producer.
            // For a more loosely-connected architecture, you can create the Consumer activity "linked" to the Producer activity as such:
            // ```
            // using var consumeActivity = ConsumerActivitySource.StartActivity("ProcessRabbitMqMessage", ActivityKind.Consumer, default(ActivityContext),
            //     links: messageContext.ActivityContext == default ? [] : [new ActivityLink(messageContext.ActivityContext)]);
            // ```
            // Note that telemetry visualization backends may or may not support navigation between *linked* activities.

            using var consumeActivity = ConsumerActivitySource.StartActivity("ProcessRabbitMqMessage", ActivityKind.Consumer, messageContext.ActivityContext);
            consumeActivity?.SetTag("messaging.system", "rabbitmq");
            consumeActivity?.SetTag("messaging.destination", queueName);
            consumeActivity?.SetTag("messaging.message.id", eventArgs.BasicProperties.MessageId);

            try
            {
                await consumeActivity.Execute(async () =>
                {
                    var report = reportGenerator.GenerateReport(eventArgs.Body);

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    logger.LogInformation("Consumed message. {Report}", report);
                });
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Failed to process RabbitMQ message {MessageId}. Nacking message.", eventArgs.BasicProperties.MessageId);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("Consuming RabbitMQ queue {QueueName} on {HostName}", queueName, hostName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static IEnumerable<string> ExtractTraceContextFromBasicProperties(IReadOnlyBasicProperties properties, string key)
    {
        if (properties.Headers is null || !properties.Headers.TryGetValue(key, out var value))
            return [];

        return value switch
        {
            string stringValue => [stringValue],
            byte[] bytes => [Encoding.UTF8.GetString(bytes)],
            _ => []
        };
    }
}
