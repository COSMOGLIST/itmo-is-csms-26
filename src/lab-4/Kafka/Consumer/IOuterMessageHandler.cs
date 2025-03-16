using Confluent.Kafka;

namespace Kafka.Consumer;

public interface IOuterMessageHandler<TKey, TValue>
{
    Task HandleAsync(IEnumerable<Message<TKey, TValue>> kafkaMessages, CancellationToken cancellationToken);
}