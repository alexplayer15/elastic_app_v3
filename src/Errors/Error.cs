using elastic_app_v3.Enums;

namespace elastic_app_v3.Errors
{
    public sealed record Error(string Code, ErrorCategory ErrorCategory, string Message)
    {
        public static readonly Error None = new(string.Empty, ErrorCategory.None, string.Empty);
    }
}
