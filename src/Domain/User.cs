namespace elastic_app_v3.Domain
{
    public sealed class User(string FirstName, string LastName, string UserName)
    {
        public Guid Id { get; set; }
        public string FirstName { get; init; } = FirstName;
        public string LastName { get; init; } = LastName;
        public string UserName { get; init; } = UserName;
        public string PasswordHash { get; set; } = string.Empty; //how to better encapsulate this while not causing too much friction in tests?
        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }

}
