namespace TelDemo.Backend.Services;

public sealed class RabbitMqConsumerWorker(ILogger<RabbitMqConsumerWorker> logger, ReportGeneratorService reportGenerator) : BackgroundService
{

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

            try
            {
                var report = await reportGenerator.GenerateReportAsync(eventArgs.Body);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: default);
                logger.LogInformation("Consumed RabbitMQ message. {Report}", report);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Failed to process RabbitMQ message {MessageId}. Nacking message.", eventArgs.BasicProperties.MessageId);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: default);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("Consuming RabbitMQ queue {QueueName} on {HostName}", queueName, hostName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
