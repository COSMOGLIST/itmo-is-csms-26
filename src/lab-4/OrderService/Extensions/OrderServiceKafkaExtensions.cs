using Confluent.Kafka;
using Kafka.Consumer;
using Kafka.Producer;
using Orders.Kafka.Contracts;
using OrderService.MessageHandlers;
using OrderService.Serialization;

namespace OrderService.Extensions;

public static class OrderServiceKafkaExtensions
{
    public static void AddOrderServiceKafka(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOuterMessageHandler<OrderProcessingKey, OrderProcessingValue>, MessageHandler>();
        serviceCollection
            .AddScoped<IKafkaProducer<OrderCreationKey, OrderCreationValue>,
                KafkaProducer<OrderCreationKey, OrderCreationValue>>();

        serviceCollection.AddScoped<IDeserializer<OrderProcessingKey>, ProtobufDeserializer<OrderProcessingKey>>();
        serviceCollection.AddScoped<IDeserializer<OrderProcessingValue>, ProtobufDeserializer<OrderProcessingValue>>();
        serviceCollection.AddScoped<ISerializer<OrderCreationKey>, ProtobufSerializer<OrderCreationKey>>();
        serviceCollection.AddScoped<ISerializer<OrderCreationValue>, ProtobufSerializer<OrderCreationValue>>();
        serviceCollection.AddHostedService<KafkaConsumerBackgroundService<OrderProcessingKey, OrderProcessingValue>>();
    }
}