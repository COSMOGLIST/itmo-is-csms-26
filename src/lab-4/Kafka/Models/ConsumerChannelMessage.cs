using Confluent.Kafka;

namespace Kafka.Models;

public class ConsumerChannelMessage<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly ConsumeResult<TKey, TValue> _message;

    public ConsumerChannelMessage(IConsumer<TKey, TValue> consumer, ConsumeResult<TKey, TValue> message)
    {
        _consumer = consumer;
        _message = message;
    }

    public Message<TKey, TValue> GetMessage()
    {
        return _message.Message;
    }

    public void MessageHandled()
    {
        _consumer.Commit(_message);
    }
}