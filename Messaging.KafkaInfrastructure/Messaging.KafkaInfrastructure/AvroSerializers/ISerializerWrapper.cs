using Confluent.Kafka;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

/// <summary>
/// Wraps generic AvroSerializer
/// </summary>
public interface ISerializerWrapper
{
    Task<byte[]> SerializeAsync(object data, SerializationContext context);
}