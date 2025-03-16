using Confluent.Kafka;
using Kafka.Models;
using System.Threading.Channels;

namespace Kafka.Consumer;

public class KafkaConsumerReader<TKey, TValue>
{
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TValue> _valueDeserializer;
    private readonly KafkaConsumerConfigOptions _kafkaConsumerConfigOptions;
    private readonly KafkaConsumerProcessOptions _kafkaConsumerProcessOptions;

    public KafkaConsumerReader(
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer,
        KafkaConsumerConfigOptions kafkaConsumerConfigOptions,
        KafkaConsumerProcessOptions kafkaConsumerProcessOptions)
    {
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
        _kafkaConsumerConfigOptions = kafkaConsumerConfigOptions;
        _kafkaConsumerProcessOptions = kafkaConsumerProcessOptions;
    }

    public async Task ReadAsync(ChannelWriter<ConsumerChannelMessage<TKey, TValue>> writer, CancellationToken cancellationToken)
    {
        await Task.Yield();

        using IConsumer<TKey, TValue> consumer = new ConsumerBuilder<TKey, TValue>(
                new ConsumerConfig
                {
                    BootstrapServers = _kafkaConsumerConfigOptions.Host,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    GroupId = _kafkaConsumerConfigOptions.GroupId,
                })
            .SetKeyDeserializer(_keyDeserializer)
            .SetValueDeserializer(_valueDeserializer)
            .Build();

        consumer.Subscribe(_kafkaConsumerProcessOptions.Topic);

        try
        {
            await ConsumeMessagesAsync(consumer, writer, cancellationToken);
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ConsumeMessagesAsync(
        IConsumer<TKey, TValue> consumer,
        ChannelWriter<ConsumerChannelMessage<TKey, TValue>> writer,
        CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            ConsumeResult<TKey, TValue> result = consumer.Consume(cancellationToken);
            var message = new ConsumerChannelMessage<TKey, TValue>(consumer, result);
            await writer.WriteAsync(message, cancellationToken);
        }
    }
}