namespace Kafka.Models;

public class KafkaConsumerProcessOptions
{
    public string Topic { get; set; } = string.Empty;

    public int ChunkSize { get; set; } = 1;
}