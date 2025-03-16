using Itmo.Dev.Platform.Common.Extensions;
using System.Threading.Channels;

namespace Task3;

public class MessageProcessor : IMessageProcessor, IMessageSender
{
    private readonly IMessageHandler _messageHandler;
    private readonly Channel<Message> _channel;

    public MessageProcessor(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
        var channelOptions = new BoundedChannelOptions(8)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
        };
        _channel = Channel.CreateBounded<Message>(channelOptions);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ChannelReader<Message> reader = _channel.Reader;
        IAsyncEnumerable<Message> messagesToWrite = reader.ReadAllAsync(cancellationToken);

        IAsyncEnumerable<IEnumerable<Message>> chunks = messagesToWrite.ChunkAsync(8, TimeSpan.Zero);
        await foreach (IEnumerable<Message> chunk in chunks)
        {
            await _messageHandler.HandleAsync(chunk, cancellationToken);
        }
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public async ValueTask SendAsync(Message message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ChannelWriter<Message> writer = _channel.Writer;
        await writer.WriteAsync(message, cancellationToken);
    }
}