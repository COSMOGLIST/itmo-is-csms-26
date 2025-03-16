using Gateway.Models.Payloads;

namespace Gateway.Mappers.PayloadMappers;

public static class ItemAddPayloadMapper
{
    public static ItemAddPayload Map(Library.ItemAddPayload itemAddPayload)
    {
        return new ItemAddPayload(itemAddPayload.OrderItemId);
    }
}