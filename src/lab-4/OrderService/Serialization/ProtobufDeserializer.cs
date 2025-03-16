using Confluent.Kafka;
using Google.Protobuf;

namespace OrderService.Serialization;

public class ProtobufDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> Parser = new(() => new T());

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull is false)
        {
            return Parser.ParseFrom(data);
        }
        else
        {
            throw new ArgumentNullException(nameof(data));
        }
    }
}