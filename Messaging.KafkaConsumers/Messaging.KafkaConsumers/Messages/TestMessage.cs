using Avro;
using Avro.Specific;

namespace Messaging.KafkaConsumers.Messages;

public class TestMessage : ISpecificRecord
{
    public readonly Schema AvroSchema = Schema.Parse(@"{""type"":""record"",""name"":""TestMessage"",""fields"":[{""name"":""TestProperty1"",""type"":""string""},{""name"":""TestProperty2"",""type"":""string""}]}");

    public virtual Schema Schema => AvroSchema;

    public string? TestProperty1 { get; set; }

    public string? TestProperty2 { get; set; }

    public virtual object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => TestProperty1,
            1 => TestProperty2,
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                TestProperty1 = (string)fieldValue;
                break;
            case 1:
                TestProperty2 = (string)fieldValue;
                break;
            default:
                throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}
