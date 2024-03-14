using MassTransit;
using Messaging.KafkaConsumers.Messages;

namespace MessagePublisher;

public class Worker
    (
        ILogger<Worker> logger,
        IServiceScopeFactory serviceScopeFactory,
        IPublisherService publisherService
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetService<ITopicProducer<string, ITaskEvent>>();
            await publisherService.Produce(producer, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Stopping");
        }
    }
}