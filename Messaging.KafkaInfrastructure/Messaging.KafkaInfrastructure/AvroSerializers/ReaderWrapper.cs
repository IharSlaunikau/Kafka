using Avro.IO;
using Avro.Specific;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

internal class ReaderWrapper<T>(Avro.Schema writerSchema, Avro.Schema readerSchema) : IReaderWrapper
{
    private readonly SpecificReader<T?> _reader = new(writerSchema, readerSchema);

    public object? Read(BinaryDecoder decoder) => _reader.Read(default, decoder);
}