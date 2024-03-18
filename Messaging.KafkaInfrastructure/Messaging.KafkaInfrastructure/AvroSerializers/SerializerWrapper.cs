using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

public class SerializerWrapper<T> : ISerializerWrapper
{
    private readonly IAsyncSerializer<T> _inner;
    private readonly ILogger<SerializerWrapper<T>> _logger;

    public SerializerWrapper(IAsyncSerializer<T> inner, ILogger<SerializerWrapper<T>> logger)
    {
        var typeOfT = typeof(T);
        if (typeOfT.IsInterface || typeOfT.IsAbstract)
        {
            throw new InvalidOperationException($"SerializerWrapper cannot handle the type {typeof(T)} because it is an interface or abstract class.");
        }

        _inner = inner;
        _logger = logger;
    }

    public async Task<byte[]> SerializeAsync(object data, SerializationContext context)
    {
        try
        {
            _logger.LogDebug($"Serializing message of type {typeof(T).Name}");

            var result = await _inner.SerializeAsync((T)data, context).ConfigureAwait(false);

            _logger.LogDebug($"Serialization for type {typeof(T).Name} has completed successfully");

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Serialization error for type {typeof(T).Name}");
            throw;
        }
    }
}