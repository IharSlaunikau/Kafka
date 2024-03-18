using Avro.Specific;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Schema = Avro.Schema;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

public abstract class MultipleTypeInfo(Type messageType, Schema schema, ILoggerFactory loggerFactory)
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public Type MessageType { get; } = messageType ?? throw new ArgumentNullException(nameof(messageType));

    public Schema Schema { get; } = schema ?? throw new ArgumentNullException(nameof(schema));

    public abstract IReaderWrapper CreateReader(Schema writerSchema);

    public abstract ISerializerWrapper CreateSerializer(ISchemaRegistryClient schemaRegistryClient,
        AvroSerializerConfig? serializerConfig);
}

public class MultipleTypeInfo<T>(Type messageType, Schema schema, ILoggerFactory loggerFactory)
    : MultipleTypeInfo(messageType, schema, loggerFactory)
    where T : ISpecificRecord
{
    public override IReaderWrapper CreateReader(Schema writerSchema) => new ReaderWrapper<T>(writerSchema, Schema);

    public override ISerializerWrapper CreateSerializer(
        ISchemaRegistryClient schemaRegistryClient, AvroSerializerConfig? serializerConfig)
    {
        var inner = new AvroSerializer<T>(schemaRegistryClient, serializerConfig);
        var logger = loggerFactory.CreateLogger<SerializerWrapper<T>>();
        return new SerializerWrapper<T>(inner, logger);
    }
}