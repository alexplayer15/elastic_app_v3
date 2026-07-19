using elastic_app_v3.domain.DTOs;
using elastic_app_v3.domain.Entities;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.domain.Abstractions;

public interface ITokenGenerator
{
    Result<JwtToken, UserError> Generate(User user);
}

