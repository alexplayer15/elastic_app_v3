namespace elastic_app_v3.Domain
{
    public sealed record User(string FirstName, string LastName, string UserName, string PasswordHash)
    {
        public string UserName { get; init; } = UserName;
        public string GetUserName() => UserName;
    }
}
