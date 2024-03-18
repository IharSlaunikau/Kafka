using Avro;
using Avro.Specific;

namespace Messaging.KafkaConsumers.Messages;

public partial class TaskCompleted : ISpecificRecord
{
    public static Schema AvroSchema = Schema.Parse(@"{""type"":""record"",""name"":""TaskCompleted"",""namespace"":""Messaging.KafkaConsumers.Messages"",""fields"":[{""name"":""Id"",""doc"":""The id of the task"",""type"":{""type"":""string"",""logicalType"":""uuid""}},{""name"":""CompletedDate"",""doc"":""The date when the task completed"",""type"":{""type"":""long"",""logicalType"":""timestamp-millis""}}]}");

    public Guid Id { get; set; }

    public DateTime CompletedDate { get; set; }

    public virtual Schema Schema => AvroSchema;

    public virtual object? Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Id;
            case 1: return CompletedDate;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: Id = (Guid)fieldValue; break;
            case 1: CompletedDate = (DateTime)fieldValue; break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        };
    }
}