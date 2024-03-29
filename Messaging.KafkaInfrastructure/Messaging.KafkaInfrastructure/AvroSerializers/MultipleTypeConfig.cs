﻿using Avro;

namespace Messaging.KafkaInfrastructure.AvroSerializers;

/// <summary>
/// Configuration used to specify the message types supported by <cref>MultipleTypeSerializer</cref> and
///  <cref>MultipleTypeDeserializer</cref>. Instances are initialized using <cref>MultipleTypeConfigBuilder</cref>.
/// </summary>
public class MultipleTypeConfig
{
    private readonly MultipleTypeInfo[] _types;

    internal MultipleTypeConfig(MultipleTypeInfo[] types)
    {
        _types = types;
    }

    public IReaderWrapper CreateReader(Schema writerSchema)
    {
        var type = _types.SingleOrDefault(x => x.Schema.Fullname == writerSchema.Fullname);

        return type == null
            ? throw new ArgumentException(
                $"Unexpected type {writerSchema.Fullname}. Supported types need to be added to this {nameof(MultipleTypeConfig)} instance",
                nameof(writerSchema))
            : type.CreateReader(writerSchema);
    }

    public IEnumerable<MultipleTypeInfo> Types => _types.AsEnumerable();
}