using Avro.IO;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

/// <summary>
/// Wraps generic Avro SpecificReader
/// </summary>
public interface IReaderWrapper
{
    object? Read(BinaryDecoder decoder);
}