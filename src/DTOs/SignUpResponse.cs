namespace elastic_app_v3.DTOs
{
    public sealed record SignUpResponse(Guid UserId)
    {
        public Guid UserId { get; init; } = UserId;
    }
}
