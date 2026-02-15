using System.ComponentModel.DataAnnotations;

namespace elastic_app_v3.infrastructure.Config
{
    public sealed class JwtConfigOptions
    {
        public const string JwtConfig = "JwtConfig";

        [Required]
        public string Issuer { get; init; } = string.Empty;

        [Required]
        public string Audience { get; init; } = string.Empty;

        [Required]
        public string PrivateKey { get; init; } = string.Empty;

        [Range(1, 1440,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int ExpirationInMinutes { get; init; }
    }
}
