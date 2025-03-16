using Gateway.Models.Payloads;

namespace Gateway.Mappers.PayloadMappers;

public static class ChangeStatePayloadMapper
{
    public static ChangeStatePayload Map(Library.ChangeStatePayload changeStatePayload)
    {
        return new ChangeStatePayload(
            OrderStateMapper.Map(changeStatePayload.PreviousState),
            OrderStateMapper.Map(changeStatePayload.NewState));
    }
}