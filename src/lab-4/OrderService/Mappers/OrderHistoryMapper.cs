using Google.Protobuf.WellKnownTypes;
using Library;

namespace OrderService.Mappers;

public static class OrderHistoryMapper
{
    public static OrderHistory Map(BasicImplementation.Models.OrderHistory orderHistory)
    {
        var newOrderHistory = new OrderHistory();
        newOrderHistory.OrderId = orderHistory.OrderId;
        newOrderHistory.OrderHistoryItemKind = OrderHistoryItemKindMapper.Map(orderHistory.OrderHistoryItemKind);
        newOrderHistory.OrderHistoryItemCreatedAt = orderHistory.OrderHistoryItemCreatedAt.ToTimestamp();
        switch (orderHistory.OrderHistoryItemPayload)
        {
            case BasicImplementation.Models.Payloads.CreatePayload:
                newOrderHistory.CreatePayload = new CreatePayload();
                break;

            case BasicImplementation.Models.Payloads.ChangeStatePayload changeStatePayload:
            {
                var newChangeStatePayload = new ChangeStatePayload
                {
                    PreviousState = OrderStateMapper.Map(changeStatePayload.PreviousState),
                    NewState = OrderStateMapper.Map(changeStatePayload.NewState),
                };
                newOrderHistory.ChangeStatePayload = newChangeStatePayload;
                break;
            }

            case BasicImplementation.Models.Payloads.ChangeStateInProcessPayload changeStateInProcessPayload:
            {
                var newChangeStateInProcessPayload = new ChangeStateInProcessPayload
                {
                    OrderProcessingEvent = changeStateInProcessPayload.OrderProcessingEvent,
                    IsSuccessful = changeStateInProcessPayload.IsSuccessful,
                };
                newOrderHistory.ChangeStateInProcessPayload = newChangeStateInProcessPayload;
                break;
            }

            case BasicImplementation.Models.Payloads.ItemAddPayload itemAddPayload:
            {
                var newItemAddPayload = new ItemAddPayload
                {
                    OrderItemId = itemAddPayload.OrderItemId,
                };
                newOrderHistory.ItemAddPayload = newItemAddPayload;
                break;
            }

            case BasicImplementation.Models.Payloads.ItemRemovePayload itemRemovePayload:
            {
                var newItemRemovePayload = new ItemRemovePayload
                {
                    OrderItemId = itemRemovePayload.OrderItemId,
                };
                newOrderHistory.ItemRemovePayload = newItemRemovePayload;
                break;
            }
        }

        return newOrderHistory;
    }
}