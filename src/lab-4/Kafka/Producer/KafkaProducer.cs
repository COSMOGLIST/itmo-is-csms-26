using Confluent.Kafka;
using Kafka.Models;
using Microsoft.Extensions.Options;

namespace Kafka.Producer;

public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly KafkaProducerProcessOptions _kafkaProducerProcessOptions;

    public KafkaProducer(
        IOptions<KafkaProducerProcessOptions> kafkaProducerProcessOptions,
        IOptions<KafkaProducerConfigOptions> kafkaProducerConfigOptions,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer)
    {
        _kafkaProducerProcessOptions = kafkaProducerProcessOptions.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaProducerConfigOptions.Value.Host,
        };
        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(KafkaProducerMessage<TKey, TValue> kafkaProducerMessage, CancellationToken cancellationToken)
    {
        var newMessage = new Message<TKey, TValue>
        {
            Key = kafkaProducerMessage.Key,
            Value = kafkaProducerMessage.Value,
        };
        await _producer.ProduceAsync(_kafkaProducerProcessOptions.Topic, newMessage, cancellationToken);
    }
}