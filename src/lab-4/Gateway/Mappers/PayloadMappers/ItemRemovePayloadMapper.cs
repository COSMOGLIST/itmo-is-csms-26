using Gateway.Models.Payloads;

namespace Gateway.Mappers.PayloadMappers;

public static class ItemRemovePayloadMapper
{
    public static ItemRemovePayload Map(Library.ItemRemovePayload itemRemovePayload)
    {
        return new ItemRemovePayload(itemRemovePayload.OrderItemId);
    }
}