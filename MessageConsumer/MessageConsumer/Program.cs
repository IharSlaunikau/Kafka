using MassTransit;
using Messaging.KafkaConsumers.Consumers;

namespace MessageConsumer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<TestConsumer>();
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}