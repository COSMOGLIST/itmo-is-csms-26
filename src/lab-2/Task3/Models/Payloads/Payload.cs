﻿using System.Text.Json.Serialization;

namespace Task3.Models.Payloads;

[JsonDerivedType(typeof(ItemRemovePayload), typeDiscriminator: nameof(ItemRemovePayload))]
[JsonDerivedType(typeof(ItemAddPayload), typeDiscriminator: nameof(ItemAddPayload))]
[JsonDerivedType(typeof(ChangeStatePayload), typeDiscriminator: nameof(ChangeStatePayload))]
[JsonDerivedType(typeof(CreatePayload), typeDiscriminator: nameof(CreatePayload))]
public record Payload;