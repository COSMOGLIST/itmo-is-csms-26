using Gateway.Models.Payloads;

namespace Gateway.Mappers.PayloadMappers;

public static class ChangeStateInProcessPayloadMapper
{
    public static ChangeStateInProcessPayload Map(Library.ChangeStateInProcessPayload changeStateInProcessPayload)
    {
        return new ChangeStateInProcessPayload(changeStateInProcessPayload.OrderProcessingEvent, changeStateInProcessPayload.IsSuccessful);
    }
}