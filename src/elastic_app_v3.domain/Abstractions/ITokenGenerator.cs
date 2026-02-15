using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Result;

namespace elastic_app_v3.domain.Abstractions
{
    public interface ITokenGenerator
    {
        Result<JwtToken> Generate(User user);
    }
}
