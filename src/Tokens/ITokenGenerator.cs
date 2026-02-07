using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Tokens
{
    public interface ITokenGenerator
    {
        Task<Result<LoginResponse>> Generate(UserSchema userSchema);
    }
}
