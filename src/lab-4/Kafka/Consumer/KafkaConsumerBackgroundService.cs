using Confluent.Kafka;
using Kafka.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Kafka.Consumer;

public class KafkaConsumerBackgroundService<TKey, TValue> : BackgroundService
{
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TValue> _valueDeserializer;
    private readonly KafkaConsumerConfigOptions _kafkaConsumerConfigOptions;
    private readonly KafkaConsumerProcessOptions _kafkaConsumerProcessOptions;
    private readonly IOuterMessageHandler<TKey, TValue> _outerMessageHandler;

    public KafkaConsumerBackgroundService(
        IOptions<KafkaConsumerConfigOptions> kafkaConsumerConfigOptions,
        IOptions<KafkaConsumerProcessOptions> kafkaConsumerProcessOptions,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer,
        IOuterMessageHandler<TKey, TValue> outerMessageHandler)
    {
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
        _outerMessageHandler = outerMessageHandler;
        _kafkaConsumerProcessOptions = kafkaConsumerProcessOptions.Value;
        _kafkaConsumerConfigOptions = kafkaConsumerConfigOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaConsumerReader = new KafkaConsumerReader<TKey, TValue>(
            _keyDeserializer,
            _valueDeserializer,
            _kafkaConsumerConfigOptions,
            _kafkaConsumerProcessOptions);

        var kafkaConsumerHandler = new KafkaConsumerHandler<TKey, TValue>(
            _kafkaConsumerProcessOptions,
            _outerMessageHandler);

        var channelOptions = new BoundedChannelOptions(_kafkaConsumerProcessOptions.ChunkSize)
        {
            SingleReader = true,
            SingleWriter = true,
        };

        var channel = Channel.CreateBounded<ConsumerChannelMessage<TKey, TValue>>(channelOptions);
        await Task.WhenAll(
            kafkaConsumerReader.ReadAsync(channel.Writer, stoppingToken),
            kafkaConsumerHandler.HandleAsync(channel.Reader, stoppingToken));
    }
}