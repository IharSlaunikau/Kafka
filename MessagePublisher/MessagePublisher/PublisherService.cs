using MassTransit;
using Messaging.KafkaConsumers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher;

public class PublisherService : IPublisherService
{
    public async Task Produce(ITopicProducer<string, ITaskEvent> producer, CancellationToken stoppingToken)
    {
        var random = new Random();

        while (true)
        {
            var startedOn = $"DEV{random.Next(1, 999).ToString().PadLeft(3, '0')}";

            var id = Guid.NewGuid();

            await ProduceMessage(id, new TaskStarted
            {
                Id = id,
                StartedDate = DateTime.Now,
                StartedOn = startedOn
            });

            await Wait(250, 500);

            await ProduceMessage(id, new TaskCompleted()
            {
                Id = id,
                CompletedDate = DateTime.Now
            });

            await Wait(1000, 2000);
        }
                
        async Task ProduceMessage(Guid key, ITaskEvent value)
        {
            if (producer == null)
            {
                Console.WriteLine("Error: producer is null");
                return;
            }

            if (value == null)
            {
                Console.WriteLine("Error: value is null");
                return;
            }

            try
            {
                await producer.Produce(key.ToString(), value, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Produce: " + ex.Message);
                Console.WriteLine("Stacktrace: " + ex.StackTrace);
                throw;
            }
        }

        async Task Wait(int min, int max) =>
            await Task.Delay(random.Next(min, max), stoppingToken);
    }
}