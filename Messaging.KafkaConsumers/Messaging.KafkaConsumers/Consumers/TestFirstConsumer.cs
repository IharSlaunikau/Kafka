using MassTransit;
using Messaging.KafkaConsumers.Messages;
using Microsoft.Extensions.Logging;

namespace Messaging.KafkaConsumers.Consumers;

public class TestFirstConsumer(ILogger<TestFirstMessage> logger) : IConsumer<TestFirstMessage>
{
    public async Task Consume(ConsumeContext<TestFirstMessage> context)
    {
        var message = context.Message;

        logger.LogInformation(string.Format("Task {0} started on {1} at {2}", message.Id, message.StartedOn,
            message.StartedDate));

        await Task.CompletedTask;
    }
}