using Kafka.Models;

namespace Kafka.Producer;

public interface IKafkaProducer<TKey, TValue>
{
    Task ProduceAsync(KafkaProducerMessage<TKey, TValue> kafkaProducerMessage, CancellationToken cancellationToken);
}