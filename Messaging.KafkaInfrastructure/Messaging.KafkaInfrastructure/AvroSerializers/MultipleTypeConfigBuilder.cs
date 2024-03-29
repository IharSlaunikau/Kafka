﻿using Avro;
using Avro.Specific;
using Microsoft.Extensions.Logging;


namespace Messaging.KafkaInfrastructure.AvroSerializers;

/// <summary>
/// Creates MultipleTypeConfig configuration object used by MultipleTypeDeserializer and
/// MultipleTypeSerializer to set supported types. This is used when reading / writing
/// different event types to the same queue.
/// </summary>
/// <typeparam name="TBase">A base type shared by all event types. This is purely to support
/// MassTransit implementation (see note in <see cref="ITaskEvent"></see>)</typeparam>
/// <summary>
/// Adds details about a type of message that can be deserialized by MultipleTypeDeserializer.
/// This must be a type generated using the avrogen.exe tool.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="readerSchema">The Avro schema used to read the object (available via the generated _SCHEMA field</param>
/// <returns></returns>
public class MultipleTypeConfigBuilder<TBase>(ILoggerFactory loggerFactory)
{
    private readonly List<MultipleTypeInfo> _types = new();

    public MultipleTypeConfigBuilder<TBase> AddType<T>(Schema readerSchema)
        where T : TBase, ISpecificRecord
    {
        ArgumentNullException.ThrowIfNull(readerSchema);

        if (_types.Any(x => x.Schema.Fullname == readerSchema.Fullname))
        {
            throw new ArgumentException($"A type based on schema with the full name \"{readerSchema.Fullname}\" has already been added");
        }
        var messageType = typeof(T);

        var mapping = new MultipleTypeInfo<T>(messageType, readerSchema, loggerFactory);

        _types.Add(mapping);

        return this;
    }

    public MultipleTypeConfig Build() => new(_types.ToArray());
}