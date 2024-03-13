namespace MessagePublisher;

public class Program
{
    private const string TaskEventsTopic = "test";
    private const string KafkaBroker = "localhost:9092";
    private const string KafkaConsumerGroup = "console-consumer-70299";

    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}