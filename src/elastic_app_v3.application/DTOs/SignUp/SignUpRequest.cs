namespace elastic_app_v3.application.DTOs.SingUp
{
    public sealed record SignUpRequest(
        string FirstName,
        string LastName,
        string UserName,
        string Password,
        string ReEnteredPassword
    );
}
