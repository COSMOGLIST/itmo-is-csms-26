using System.Text;

namespace Task3;

public class MessageHandler : IMessageHandler
{
    public async ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var stringBuilder = new StringBuilder();
        foreach (Message message in messages)
        {
            stringBuilder.Append(message.Title).Append('\n').Append(message.Text).Append('\n');
        }

        await Console.Out.WriteLineAsync(stringBuilder.ToString());
    }
}