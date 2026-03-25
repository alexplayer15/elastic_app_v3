namespace elastic_app_v3.domain.Events;
public sealed record UserSignedUp(Guid UserId) : Event;
