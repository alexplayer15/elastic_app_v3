using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions
{
    public interface ITokenGenerator
    {
        Result<JwtToken> Generate(User user);
    }
}
