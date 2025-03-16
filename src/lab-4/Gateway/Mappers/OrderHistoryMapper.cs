using Gateway.Mappers.PayloadMappers;
using Gateway.Models;
using Gateway.Models.Payloads;
using Library;

namespace Gateway.Mappers;

public static class OrderHistoryMapper
{
    public static OrderHistoryDto Map(OrderHistory orderHistory)
    {
        long orderHistoryId = orderHistory.Id;
        long orderId = orderHistory.OrderId;
        Models.OrderHistoryItemKind orderHistoryItemKind = OrderHistoryItemKindMapper.MapToModel(orderHistory.OrderHistoryItemKind);
        var orderHistoryItemCreatedAt = orderHistory.OrderHistoryItemCreatedAt.ToDateTime();
        Payload? payload = orderHistory.OderHistoryItemPayloadCase switch
        {
            OrderHistory.OderHistoryItemPayloadOneofCase.ChangeStatePayload => ChangeStatePayloadMapper.Map(orderHistory.ChangeStatePayload),
            OrderHistory.OderHistoryItemPayloadOneofCase.ChangeStateInProcessPayload => ChangeStateInProcessPayloadMapper.Map(orderHistory.ChangeStateInProcessPayload),
            OrderHistory.OderHistoryItemPayloadOneofCase.CreatePayload => CreatePayloadMapper.Map(orderHistory.CreatePayload),
            OrderHistory.OderHistoryItemPayloadOneofCase.ItemAddPayload => ItemAddPayloadMapper.Map(orderHistory.ItemAddPayload),
            OrderHistory.OderHistoryItemPayloadOneofCase.ItemRemovePayload => ItemRemovePayloadMapper.Map(orderHistory.ItemRemovePayload),
            OrderHistory.OderHistoryItemPayloadOneofCase.None => null,
            _ => throw new Exception("Unknown payload type"),
        };

        return new OrderHistoryDto(orderHistoryId, orderId, orderHistoryItemCreatedAt, orderHistoryItemKind, payload);
    }
}