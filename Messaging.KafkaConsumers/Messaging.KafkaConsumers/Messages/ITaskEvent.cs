namespace Messaging.KafkaConsumers.Messages;

public interface ITaskEvent
{
    Guid Id { get; }
}

public partial class TestFirstMessage : ITaskEvent;

public partial class TestSecondMessage : ITaskEvent;