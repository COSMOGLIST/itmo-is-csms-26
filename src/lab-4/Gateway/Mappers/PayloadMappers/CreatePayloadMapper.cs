using Gateway.Models.Payloads;

namespace Gateway.Mappers.PayloadMappers;

public static class CreatePayloadMapper
{
    public static CreatePayload Map(Library.CreatePayload createPayload)
    {
        return new CreatePayload();
    }
}