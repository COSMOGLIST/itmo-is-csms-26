using Confluent.Kafka;
using Itmo.Dev.Platform.Common.Extensions;
using Kafka.Models;
using System.Threading.Channels;

namespace Kafka.Consumer;

public class KafkaConsumerHandler<TKey, TValue>
{
    private readonly KafkaConsumerProcessOptions _kafkaConsumerProcessOptions;
    private readonly IOuterMessageHandler<TKey, TValue> _outerMessageHandler;

    public KafkaConsumerHandler(
        KafkaConsumerProcessOptions kafkaConsumerProcessOptions,
        IOuterMessageHandler<TKey, TValue> outerMessageHandler)
    {
        _kafkaConsumerProcessOptions = kafkaConsumerProcessOptions;
        _outerMessageHandler = outerMessageHandler;
    }

    public async Task HandleAsync(ChannelReader<ConsumerChannelMessage<TKey, TValue>> reader, CancellationToken cancellationToken)
    {
        await Task.Yield();

        IAsyncEnumerable<IReadOnlyList<ConsumerChannelMessage<TKey, TValue>>> allChunks =
            reader.ReadAllAsync(cancellationToken)
            .ChunkAsync(_kafkaConsumerProcessOptions.ChunkSize, TimeSpan.Zero);

        await foreach (IReadOnlyList<ConsumerChannelMessage<TKey, TValue>> chunk in allChunks)
        {
            IEnumerable<Message<TKey, TValue>> oneChunk = chunk.Select(message => message.GetMessage());
            await _outerMessageHandler.HandleAsync(oneChunk, cancellationToken);
            foreach (ConsumerChannelMessage<TKey, TValue> message in chunk)
            {
                message.MessageHandled();
            }
        }
    }
}