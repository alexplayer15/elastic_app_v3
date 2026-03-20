namespace elastic_app_v3.domain.Entities
{
    public sealed record IdempotentPaymentData(
        string IdempotencyKey,
        Guid PaymentId
    );
}
