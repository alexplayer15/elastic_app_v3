namespace elastic_app_v3.DTOs
{
    public sealed record GetUserResponse(string FirstName, string LastName, string UserName)
    {
        public string FirstName { get; init; } = FirstName;
        public string LastName { get; init; } = LastName;
        public string UserName { get; init; } = UserName;
    }
}
