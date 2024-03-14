using MassTransit;

namespace MessageConsumer;

public class Worker(ILogger<Worker> logger, IBusControl busControl) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await busControl.StartAsync(stoppingToken);
        logger.LogInformation("Worker running, listening to Kafka Topic");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken); // wait for 5 seconds in each iteration
        }

        await busControl.StopAsync(stoppingToken);
    }
}