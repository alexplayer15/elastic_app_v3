namespace elastic_app_v3.application.DTOs
{
    public sealed record GetUserResponse(
        string FirstName, 
        string LastName, 
        string UserName
    );
}
