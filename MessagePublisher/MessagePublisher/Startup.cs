//using Confluent.Kafka.SyncOverAsync;
//using Confluent.SchemaRegistry;
//using Confluent.SchemaRegistry.Serdes;
//using MassTransit;
//using Messaging.KafkaConsumers.Messages;
//using Messaging.KafkaInfrastructure.AvroSerializers;

//namespace MessagePublisher;

//public class Startup
//{
//    private const string TaskEventsTopic = "task-events";
//    private const string KafkaBroker = "localhost:9092";
//    private const string SchemaRegistryUrl = "http://localhost:8081";

//    private async Task ConfigureServices(IServiceCollection services)
//    {
//        var schemaRegistryConfig = new SchemaRegistryConfig { Url = SchemaRegistryUrl };
//        var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

//        var avroSchema = Avro.Schema.Parse(await File.ReadAllTextAsync("Properties\\testSchema.avsc"));

//        var schemaId = await schemaRegistryClient.RegisterSchemaAsync(TaskEventsTopic, avroSchema.ToString());

//        services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);

//        services.AddMassTransit(busConfig =>
//        {
//            busConfig.UsingInMemory((context, config) => config.ConfigureEndpoints(context));

//            busConfig.AddRider(riderConfig =>
//            {
//                var multipleTypeConfig = new MultipleTypeConfigBuilder<ITaskEvent>()
//                    .AddType<TestFirstMessage>(TestFirstMessage.AvroSchema)
//                    .AddType<TestFirstMessage>(TestSecondMessage.AvroSchema)
//                    .Build();

//                riderConfig.AddProducer<string, ITaskEvent>(TaskEventsTopic, (riderContext, producerConfig) =>
//                {
//                    var serializerConfig = new AvroSerializerConfig
//                    {
//                        SubjectNameStrategy = SubjectNameStrategy.Record,
//                        AutoRegisterSchemas = true
//                    };

//                    var serializer = new MultipleTypeSerializer<ITaskEvent>(multipleTypeConfig, schemaRegistryClient, serializerConfig);

//                    producerConfig.SetKeySerializer(new AvroSerializer<string>(schemaRegistryClient).AsSyncOverAsync());
//                    producerConfig.SetValueSerializer(serializer.AsSyncOverAsync());
//                });

//                riderConfig.UsingKafka((_, kafkaConfig) =>
//                {
//                    kafkaConfig.Host(KafkaBroker);
//                });
//            });
//        });

//        // This service produces the events
//        services.AddHostedService<Worker>();
//    }
//}
