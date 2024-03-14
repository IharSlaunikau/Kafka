using MassTransit;
using Messaging.KafkaConsumers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher
{
    public interface IPublisherService
    {
        Task Produce(ITopicProducer<string, ITaskEvent> producer, CancellationToken stoppingToken);
    }
}
