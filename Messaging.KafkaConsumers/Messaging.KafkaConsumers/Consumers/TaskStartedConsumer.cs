using System;
using System.Threading.Tasks;
using Avro;
using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Messaging.KafkaConsumers.Messages;
using Microsoft.Extensions.Logging;

namespace Messaging.KafkaConsumers.Consumers
{
    public class TaskStartedConsumer(ILogger<TaskStartedConsumer> logger) : IConsumer<TaskStarted>
    {
        public async Task Consume(ConsumeContext<TaskStarted> context)
        {
            var ctx = context.ReceiveContext as KafkaReceiveContext<Ignore, TaskStarted>;
            Console.WriteLine($"Message: {context.Message.Id}, Offset: {ctx?.Offset}");

            var message = context.Message;
            logger.LogInformation($"Task {message.Id} started on {message.StartedOn} at {message.StartedDate}");
            await Task.CompletedTask;
        }
    }
}
