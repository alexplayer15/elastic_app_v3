namespace elastic_app_v3.DTOs
{
    public sealed record SignUpRequest(
        string FirstName,
        string LastName,
        string UserName,
        string Password,
        string ReEnteredPassword
    );
}
