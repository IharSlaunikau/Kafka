using Messaging.KafkaConsumers.Messages;
using MassTransit;

namespace Messaging.KafkaConsumers.Consumers;

public class TestConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        Console.WriteLine($"Received TestMessage. TestProperty1={context.Message.TestProperty1}, TestProperty2={context.Message.TestProperty2}");

        return Task.CompletedTask;
    }
}