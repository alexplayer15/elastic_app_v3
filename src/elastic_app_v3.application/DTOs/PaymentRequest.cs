namespace elastic_app_v3.application.DTOs
{
    public sealed record PaymentRequest(
        double Amount,
        string Currency,
        string Status,
        string Description
    );
}
