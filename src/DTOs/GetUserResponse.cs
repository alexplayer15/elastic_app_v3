namespace elastic_app_v3.DTOs
{
    public sealed record GetUserResponse(
        string FirstName, 
        string LastName, 
        string UserName,
        DateTime CreatedAt
    );

}
