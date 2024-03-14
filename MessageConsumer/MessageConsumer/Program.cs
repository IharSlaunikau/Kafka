using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using Messaging.KafkaConsumers.Consumers;
using Messaging.KafkaConsumers.Messages;

namespace MessageConsumer;

public class Program
{
    public static async Task Main(string[] args)
    {

        try
        {

        }
        catch (Exception a)
        {
            Console.WriteLine(a);
            throw;
        }
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var kafkaConfig = hostContext.Configuration.GetSection("Kafka");

                var schemaRegistryConfig = new SchemaRegistryConfig { Url = kafkaConfig["SchemaRegistryUrl"] };
                var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

                var consumerConfig = new ConsumerConfig
                {
                    GroupId = kafkaConfig["KafkaGroupId"],
                    BootstrapServers = kafkaConfig["BootstrapServers"],
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                services.AddSingleton(p => new ConsumerBuilder<Ignore, TestFirstMessage>(consumerConfig)
                    .SetValueDeserializer(new AvroDeserializer<TestFirstMessage>(schemaRegistryClient).AsSyncOverAsync())
                    .Build());

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<TestConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });

                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<TestConsumer>();

                        rider.UsingKafka((context, k) =>
                        {
                            k.Host(kafkaConfig["KafkaBroker"]);

                            k.TopicEndpoint<TestFirstMessage>(kafkaConfig["TaskEventsTopic"], kafkaConfig["KafkaGroupId"], e =>
                            {
                                e.ConfigureConsumer<TestConsumer>(context);
                            });
                        });
                    });

                });

                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}