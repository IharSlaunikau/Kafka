using MassTransit;
using Messaging.KafkaConsumers.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.KafkaConsumers.Consumers
{
    public class TaskEventConsumer(ILogger<TaskEventConsumer> logger) : IConsumer<ITaskEvent>
    {
        public async Task Consume(ConsumeContext<ITaskEvent> context)
        {
            void LogEvent(string description) =>
                logger.LogInformation("{0:O} Received ({1} {2}) event: {3}", "", context.Message.Id, context.Message.GetType(), description);

            var message = context.Message;

            switch (message)
            {
                case TaskStarted started:
                    LogEvent($"Task started by {started.StartedOn} at {started.StartedDate}");
                    break;
                case TaskCompleted started:
                    LogEvent($"Task completed at {started.CompletedDate}");
                    break;
            }

            await Task.CompletedTask;
        }
    }
}
