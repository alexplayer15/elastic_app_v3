namespace elastic_app_v3.domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; //how to better encapsulate this while not causing too much friction in tests?
        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }

}
