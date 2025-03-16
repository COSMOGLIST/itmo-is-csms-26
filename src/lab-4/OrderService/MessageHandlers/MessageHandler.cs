using BasicImplementation.Models;
using BasicImplementation.Models.Payloads;
using BasicImplementation.Services;
using Confluent.Kafka;
using Kafka.Consumer;
using Orders.Kafka.Contracts;

namespace OrderService.MessageHandlers;

public class MessageHandler : IOuterMessageHandler<OrderProcessingKey, OrderProcessingValue>
{
    private readonly IOrderService _orderService;
    private readonly IOrderHistoryService _orderHistoryService;

    public MessageHandler(IOrderService orderService, IOrderHistoryService orderHistoryService)
    {
        _orderService = orderService;
        _orderHistoryService = orderHistoryService;
    }

    public async Task HandleAsync(
        IEnumerable<Message<OrderProcessingKey, OrderProcessingValue>> kafkaMessages,
        CancellationToken cancellationToken)
    {
        foreach (Message<OrderProcessingKey, OrderProcessingValue> kafkaMessage in kafkaMessages)
        {
            switch (kafkaMessage.Value)
            {
                case { ApprovalReceived: not null }:
                    var orderHistoryApproval = new OrderHistory(
                        kafkaMessage.Value.ApprovalReceived.OrderId,
                        kafkaMessage.Value.ApprovalReceived.CreatedAt.ToDateTime(),
                        OrderHistoryItemKind.StateChangedInProcess,
                        new ChangeStateInProcessPayload(
                            "ApprovalReceived",
                            kafkaMessage.Value.ApprovalReceived.IsApproved));
                    await _orderHistoryService.CreateAsync(orderHistoryApproval, cancellationToken);

                    if (kafkaMessage.Value.ApprovalReceived.IsApproved is false)
                    {
                        await _orderService.CancelOrderPrivilegedAsync(kafkaMessage.Value.ApprovalReceived.OrderId, cancellationToken);
                    }

                    break;

                case { PackingStarted: not null }:
                    var orderHistoryPackingStarted = new OrderHistory(
                        kafkaMessage.Value.PackingStarted.OrderId,
                        kafkaMessage.Value.PackingStarted.StartedAt.ToDateTime(),
                        OrderHistoryItemKind.StateChangedInProcess,
                        new ChangeStateInProcessPayload(
                            "PackingStarted",
                            true));
                    await _orderHistoryService.CreateAsync(orderHistoryPackingStarted, cancellationToken);

                    break;

                case { PackingFinished: not null }:
                    var orderHistoryPackingFinished = new OrderHistory(
                        kafkaMessage.Value.PackingFinished.OrderId,
                        kafkaMessage.Value.PackingFinished.FinishedAt.ToDateTime(),
                        OrderHistoryItemKind.StateChangedInProcess,
                        new ChangeStateInProcessPayload(
                            "PackingFinished",
                            kafkaMessage.Value.PackingFinished.IsFinishedSuccessfully));
                    await _orderHistoryService.CreateAsync(orderHistoryPackingFinished, cancellationToken);

                    if (kafkaMessage.Value.PackingFinished.IsFinishedSuccessfully is false)
                    {
                        await _orderService.CancelOrderPrivilegedAsync(kafkaMessage.Value.PackingFinished.OrderId, cancellationToken);
                    }

                    break;

                case { DeliveryStarted: not null }:
                    var orderHistoryDeliveryStarted = new OrderHistory(
                        kafkaMessage.Value.DeliveryStarted.OrderId,
                        kafkaMessage.Value.DeliveryStarted.StartedAt.ToDateTime(),
                        OrderHistoryItemKind.StateChangedInProcess,
                        new ChangeStateInProcessPayload(
                            "DeliveryStarted",
                            true));
                    await _orderHistoryService.CreateAsync(orderHistoryDeliveryStarted, cancellationToken);

                    break;

                case { DeliveryFinished: not null }:
                    var orderHistoryDeliveryFinished = new OrderHistory(
                        kafkaMessage.Value.DeliveryFinished.OrderId,
                        kafkaMessage.Value.DeliveryFinished.FinishedAt.ToDateTime(),
                        OrderHistoryItemKind.StateChangedInProcess,
                        new ChangeStateInProcessPayload(
                            "DeliveryFinished",
                            kafkaMessage.Value.DeliveryFinished.IsFinishedSuccessfully));
                    await _orderHistoryService.CreateAsync(orderHistoryDeliveryFinished, cancellationToken);

                    if (kafkaMessage.Value.DeliveryFinished.IsFinishedSuccessfully is false)
                    {
                        await _orderService.CancelOrderPrivilegedAsync(kafkaMessage.Value.DeliveryFinished.OrderId, cancellationToken);
                    }
                    else
                    {
                        await _orderService.CompleteOrderAsync(kafkaMessage.Value.DeliveryFinished.OrderId, cancellationToken);
                    }

                    break;
            }
        }
    }
}