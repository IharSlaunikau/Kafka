using MassTransit;
using Messaging.KafkaConsumers.Messages;
using Microsoft.Extensions.Logging;

namespace Messaging.KafkaConsumers.Consumers;

public class TestSecondConsumer(ILogger<TestSecondMessage> logger) : IConsumer<TestSecondMessage>
{
    public async Task Consume(ConsumeContext<TestSecondMessage> context)
    {
        var message = context.Message;

        logger.LogInformation(string.Format("Task {0} started on {1} at {2}", message.Id, message.StartedOn,
            message.StartedDate));

        await Task.CompletedTask;
    }
}