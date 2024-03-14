using Avro;
using Avro.Specific;

namespace Messaging.KafkaConsumers.Messages;

public partial class TestFirstMessage : ISpecificRecord
{
    public static Schema AvroSchema = Avro.Schema.Parse(
    @"{
     ""type"": ""record"",
     ""name"": ""TaskTest"",
     ""namespace"": ""Messaging.KafkaConsumers.Messages"",
     ""fields"": [
        {
          ""name"": ""Id"",
          ""doc"": ""The id of the task"",
          ""type"": {
             ""type"": ""string"",
             ""logicalType"": ""uuid""
          }
        },
        {
          ""name"": ""StartedDate"",
          ""doc"": ""The date when the task started"",
          ""type"": {
             ""type"": ""long"",
             ""logicalType"": ""timestamp-millis""
          }
        },
        {
          ""name"": ""StartedOn"",
          ""doc"": ""The worker starting the task"",
          ""type"": ""string""
        }
       ]
    }");

    public Guid Id { get; set; }

    public DateTime StartedDate { get; set; }

    public string? StartedOn { get; set; }

    public virtual Schema Schema => AvroSchema;

    public virtual object? Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Id;
            case 1: return StartedDate;
            case 2: return StartedOn;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: Id = (Guid)fieldValue; break;
            case 1: StartedDate = (DateTime)fieldValue; break;
            case 2: StartedOn = fieldValue as string; break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        };
    }
}
