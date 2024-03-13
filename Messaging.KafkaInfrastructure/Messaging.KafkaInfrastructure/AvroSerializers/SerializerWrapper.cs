using Confluent.Kafka;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

internal class SerializerWrapper<T>(IAsyncSerializer<T> inner) : ISerializerWrapper
{
    public async Task<byte[]> SerializeAsync(object data, SerializationContext context)
    {
        return await inner.SerializeAsync((T)data, context).ConfigureAwait(false);
    }
}