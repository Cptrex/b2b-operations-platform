namespace Platform.Identity.Abstractions;

public sealed record ActorContext
(
    string? ActorId,
    string? Issuer,
    string? UserName,
    ActorType Type
);