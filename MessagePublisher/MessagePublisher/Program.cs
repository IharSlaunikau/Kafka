
using System.Reflection;
using MessagePublisher.Extensions;

namespace MessagePublisher;

public class Program
{
    public static async Task Main(string[] args)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        Console.WriteLine($@"Message Publisher {version} starting...");

        var worker = CreateHostBuilder(args).Build().Services.GetService<Worker>();

        if (worker == null)
        {
            throw new InvalidOperationException("Worker not found in services");
        }

        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => InitializeConfiguration(builder, args))
            .ConfigureServices(RegisterServices);

    private static void InitializeConfiguration(IConfigurationBuilder builder, string[] args)
    {
        builder.AddJsonFile("appsettings.json", false, true);
        builder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json",
            false, true);

        // environment variables
        builder.AddEnvironmentVariables();

        // add main args
        builder.AddCommandLine(args);

        var services = new ServiceCollection();
        var configuration = builder.Build();
        services.AddLogging();
    }

    private static void RegisterServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddLogging();

        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

        _ = ServiceResolver.RegisterSchemaAsync();

        services.RegisterSchemaRegistryClient();
        services.RegisterMassTransit(loggerFactory);

        services.AddSingleton<Worker>();
        services.RegisterBusinessServices(hostContext.Configuration);

        services.AddHostedService<Worker>();
    }
}


//var host = Host.CreateDefaultBuilder(args)
//           .ConfigureAppConfiguration((_, config) =>
//           {
//               config.SetBasePath(Directory.GetCurrentDirectory())
//                   .AddJsonFile("appsettings.json", optional: false);
//           })
//           .ConfigureServices(async (hostContext, services) =>
//           {
//               var kafkaConfig = hostContext.Configuration.GetSection("Kafka");

//               var schemaRegistryConfig = new SchemaRegistryConfig { Url = kafkaConfig["SchemaRegistryUrl"] };
//               var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
//               var avroSchema = Avro.Schema.Parse(await File.ReadAllTextAsync("Properties\\testSchema.avsc"));

//               var schemaId = await schemaRegistryClient.RegisterSchemaAsync(kafkaConfig["TaskEventsTopic"], avroSchema.ToString());

//               var producerConfig = new ProducerConfig
//               {
//                   BootstrapServers = kafkaConfig["BootstrapServers"]
//               };

//               services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);

//               services.AddSingleton(p => new ProducerBuilder<Null, TestMessage>(producerConfig)
//                   .SetValueSerializer(new AvroSerializer<TestMessage>(schemaRegistryClient))
//                   .Build());

//               services.AddMassTransit(x =>
//               {
//                   x.AddConsumer<TestConsumer>();

//                   x.UsingInMemory((context, cfg) =>
//                   {
//                       cfg.ConfigureEndpoints(context);
//                   });

//                   x.AddRider(rider =>
//                   {
//                       rider.AddConsumer<TestConsumer>();

//                       rider.UsingKafka((context, k) =>
//                       {
//                           k.Host(kafkaConfig["KafkaBroker"]);

//                           k.TopicEndpoint<TestMessage>(kafkaConfig["TaskEventsTopic"], kafkaConfig["KafkaGroupId"], e =>
//                           {
//                               e.ConfigureConsumer<TestConsumer>(context);
//                           });
//                       });
//                   });

//               });

//               services.AddHostedService<Worker>();
//           })
//           .Build();

//await host.RunAsync();

