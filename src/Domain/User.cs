namespace elastic_app_v3.Domain
{
    public sealed record User(string FirstName, string LastName, string UserName, string Password)
    {
        public Guid UserId { get; init; } = Guid.NewGuid();
        public string FirstName { get; init; } = FirstName;
        public string LastName { get; init; } = LastName;
        public string UserName { get; init; } = UserName;
        public string Password { get; init; } = Password;
        public string GetUserName() => UserName;
        public Guid GetId() => UserId;
    }
}
