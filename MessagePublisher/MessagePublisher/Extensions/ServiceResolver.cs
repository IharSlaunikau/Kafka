using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using Messaging.KafkaConsumers.Consumers;
using Messaging.KafkaConsumers.Messages;
using Messaging.KafkaInfrastructure.AvroSerializers;
using Microsoft.Extensions.Logging;

namespace MessagePublisher.Extensions;

public static class ServiceResolver
{
    private const string TaskEventsTopic = "task-events";
    private const string KafkaBroker = "host.docker.internal:9092";
    private const string SchemaRegistryUrl = "http://host.docker.internal:8081";

    public static void RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPublisherService, PublisherService>();
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
    }

    public static async Task RegisterSchemaAsync()
    {
        var schemaRegistryClient = GetSchemaRegistryClient();

        var schema = await File.ReadAllTextAsync("Properties\\TaskTest.avsc");
        var schema2 = await File.ReadAllTextAsync("Properties\\TaskTesе2.avsc");

        var avroSchema = Avro.Schema.Parse(schema);
        var avroSchema2 = Avro.Schema.Parse(schema2);

        try
        {
            var schemaId = await schemaRegistryClient.RegisterSchemaAsync(TaskEventsTopic, avroSchema.ToString());
            Console.WriteLine($"Schema with ID {schemaId} has been successfully registered for {TaskEventsTopic}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to register schema for {TaskEventsTopic}. Error: {e.Message}");
        }

        await Task.Delay(1000);

        try
        {

            var schemaId2 = await schemaRegistryClient.RegisterSchemaAsync(TaskEventsTopic, avroSchema2.ToString());
            Console.WriteLine($"Schema with ID {schemaId2} has been successfully registered for {TaskEventsTopic}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to register schema for {TaskEventsTopic}. Error: {e.Message}");
        }
    }

    public static void RegisterSchemaRegistryClient(this IServiceCollection services)
    {
        var schemaRegistryClient = GetSchemaRegistryClient();

        if (schemaRegistryClient == null)
        {
            throw new Exception("SchemaRegistryClient is not initialized");
        }

        services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);
    }

    public static void RegisterMassTransit(this IServiceCollection services, ILoggerFactory loggerFactory)
    {
        var schemaRegistryClient = GetSchemaRegistryClient();

        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<TaskStartedConsumer>();
            busConfig.AddConsumer<TaskCompletedConsumer>();
            busConfig.AddConsumer<TaskEventConsumer>();

            busConfig.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
            busConfig.AddRider(riderConfig =>
            {
                riderConfig.AddConsumer<TaskStartedConsumer>();
                riderConfig.AddConsumer<TaskCompletedConsumer>();
                riderConfig.AddConsumer<TaskEventConsumer>();

                // Specify supported message types here. Support is restricted to types generated via avrogen.exe
                // tool. Being explicit makes this a lot simpler as we can use Avro Schema objects rather than messing
                // around with .NET Types / reflection.
                var multipleTypeConfig = new MultipleTypeConfigBuilder<ITaskEvent>(loggerFactory)
                    .AddType<TaskStarted>(TaskStarted.AvroSchema)
                    .AddType<TaskCompleted>(TaskCompleted.AvroSchema)
                    .Build();


                // Set up producers - events are produced by DemoProducer hosted service
                riderConfig.AddProducer<string, ITaskEvent>(TaskEventsTopic, (riderContext, producerConfig) =>
                {
                    // Serializer configuration.

                    // Important: Use either SubjectNameStrategy.Record or SubjectNameStrategy.TopicRecord.
                    // SubjectNameStrategy.Topic (default) would result in the topic schema being set based on
                    // the first message produced.
                    //
                    // Note that you can restrict the range of message types for a topic by setting up the
                    // topic schema using schema references. This hasn't yet been covered in this demo - more
                    // details available here:
                    // https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#multiple-event-types-in-the-same-topic
                    // https://docs.confluent.io/platform/current/schema-registry/serdes-develop/serdes-avro.html#multiple-event-types-same-topic-avro
                    // https://www.confluent.io/blog/multiple-event-types-in-the-same-kafka-topic/
                    var serializerConfig = new AvroSerializerConfig
                    {
                        SubjectNameStrategy = SubjectNameStrategy.Record,
                        AutoRegisterSchemas = true
                    };

                    var serializer = new MultipleTypeSerializer<ITaskEvent>(multipleTypeConfig, schemaRegistryClient, serializerConfig);
                    // Note that all child serializers share the same AvroSerializerConfig - separate producers could
                    // be used for each logical set of message types (e.g. all messages produced to a certain topic)
                    // to support varying configuration if needed.
                    producerConfig.SetKeySerializer(new AvroSerializer<string>(schemaRegistryClient).AsSyncOverAsync());
                    producerConfig.SetValueSerializer(serializer.AsSyncOverAsync());


                });

                // Set up consumers and consuming
                riderConfig.AddConsumersFromNamespaceContaining<TaskStarted>();

                riderConfig.UsingKafka((riderContext, kafkaConfig) =>
                {
                    kafkaConfig.Host(KafkaBroker);
                    var groupId = Guid.NewGuid().ToString(); // always start from beginning
                    kafkaConfig.TopicEndpoint<string, ITaskEvent>(TaskEventsTopic, groupId, topicConfig =>
                    {
                        topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                        topicConfig.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistryClient, null).AsSyncOverAsync());
                        topicConfig.SetValueDeserializer(
                            new MultipleTypeDeserializer<ITaskEvent>(multipleTypeConfig, schemaRegistryClient)
                                .AsSyncOverAsync());
                        topicConfig.ConfigureConsumer<TaskStartedConsumer>(riderContext);
                        topicConfig.ConfigureConsumer<TaskCompletedConsumer>(riderContext);

                        // Example of consuming base message type and being able to work with
                        // concrete subclass
                        topicConfig.ConfigureConsumer<TaskEventConsumer>(riderContext);
                    });
                });
            });
        });
    }

    private static CachedSchemaRegistryClient GetSchemaRegistryClient()
    {
        var schemaRegistryConfig = new SchemaRegistryConfig { Url = SchemaRegistryUrl };
        var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

        return schemaRegistryClient;
    }

}