using Task3;

var implementation = new MessageProcessor(new MessageHandler());

MessageProcessor processor = implementation;
MessageProcessor sender = implementation;
var cancellationTokenSource = new CancellationTokenSource();
CancellationToken cancellationToken = cancellationTokenSource.Token;

Task mainTask = processor.ProcessAsync(cancellationToken);

Message[] messages =
{
    new Message("Tittle1", "Text1"),
    new Message("Tittle2", "Text2"),
    new Message("Tittle3", "Text3"),
    new Message("Tittle4", "Text4"),
};

var tasks = new List<Task>();
Parallel.ForEach(messages, message =>
{
    ValueTask task = sender.SendAsync(message, cancellationToken);
    tasks.Add(task.AsTask());
});

await Task.WhenAll(tasks);

processor.Complete();

await mainTask;