using System.Text.Json.Serialization;

namespace Gateway.Models.Payloads;

/// <summary>
/// Order history payload
/// </summary>
[JsonDerivedType(typeof(ItemRemovePayload), typeDiscriminator: nameof(ItemRemovePayload))]
[JsonDerivedType(typeof(ItemAddPayload), typeDiscriminator: nameof(ItemAddPayload))]
[JsonDerivedType(typeof(ChangeStatePayload), typeDiscriminator: nameof(ChangeStatePayload))]
[JsonDerivedType(typeof(CreatePayload), typeDiscriminator: nameof(CreatePayload))]
[JsonDerivedType(typeof(ChangeStateInProcessPayload), typeDiscriminator: nameof(ChangeStateInProcessPayload))]
public record Payload;