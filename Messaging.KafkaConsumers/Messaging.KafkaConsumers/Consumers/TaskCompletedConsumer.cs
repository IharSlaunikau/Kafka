using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Messaging.KafkaConsumers.Messages;
using Microsoft.Extensions.Logging;

namespace Messaging.KafkaConsumers.Consumers;

public class TaskCompletedConsumer(ILogger<TaskCompleted> logger) : IConsumer<TaskCompleted>
{
    public async Task Consume(ConsumeContext<TaskCompleted> context)
    {
        var ctx = (context.ReceiveContext as KafkaReceiveContext<Guid, ITaskEvent>);
        Console.WriteLine($"Message: {context.Message.Id}, Offset: {ctx?.Offset}");

        var message = context.Message;

        logger.LogInformation(string.Format("Task {0} completed on {1}", message.Id, message.CompletedDate));

        await Task.CompletedTask;
    }
}