using Avro;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using Messaging.KafkaConsumers.Consumers;
using Messaging.KafkaConsumers.Messages;

namespace MessageConsumer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddRider(rider =>
                    {
                        rider.UsingKafka((context, k) =>
                        {
                            k.Host(hostContext.Configuration.GetSection("Kafka:Brokers").Get<string[]>());
                        });
                    });
                });

                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}