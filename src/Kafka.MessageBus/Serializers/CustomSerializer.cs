using System.Text;
using Avro.IO;
using Avro.Specific;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Kafka.MessageBus.Serializers;

public class CustomSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        var str=System.Text.Json.JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(str);
    }
}

public class CustomDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(data) ?? throw new InvalidOperationException();
    }
}