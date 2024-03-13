using System;
using Avro.Specific;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Schema = Avro.Schema;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

public abstract class MultipleTypeInfo(Type messageType, Schema schema)
{
    public Type MessageType { get; } = messageType ?? throw new ArgumentNullException(nameof(messageType));

    /// <summary>
    /// The schema used to serialize / deserialize within this application
    /// </summary>
    public Schema Schema { get; } = schema ?? throw new ArgumentNullException(nameof(schema));

    public abstract IReaderWrapper CreateReader(Schema writerSchema);

    public abstract ISerializerWrapper CreateSerializer(ISchemaRegistryClient schemaRegistryClient,
        AvroSerializerConfig? serializerConfig);
}

public class MultipleTypeInfo<T>(Type messageType, Schema schema) : MultipleTypeInfo(messageType, schema)
    where T : ISpecificRecord
{
    public override IReaderWrapper CreateReader(Schema writerSchema) => new ReaderWrapper<T>(writerSchema, Schema);

    public override ISerializerWrapper CreateSerializer(ISchemaRegistryClient schemaRegistryClient, AvroSerializerConfig? serializerConfig)
    {
        var inner = new AvroSerializer<T>(schemaRegistryClient, serializerConfig);
        return new SerializerWrapper<T>(inner);
    }
}