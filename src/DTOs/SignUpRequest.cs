namespace elastic_app_v3.DTOs
{
    public sealed record SignUpRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReEnteredPassword { get; set; }
    }
}
