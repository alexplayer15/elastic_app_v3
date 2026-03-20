namespace elastic_app_v3.infrastructure.Models
{
    public sealed record IdempotencyKeySchema(
        string IdempotencyKey,
        Guid PaymentId,
        DateTime CreatedAt
    );
}
