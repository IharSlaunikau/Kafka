namespace Messaging.KafkaConsumers.Messages;

public interface ITaskEvent
{
    Guid Id { get; }
}

public partial class TaskStarted : ITaskEvent;

public partial class TaskCompleted : ITaskEvent;