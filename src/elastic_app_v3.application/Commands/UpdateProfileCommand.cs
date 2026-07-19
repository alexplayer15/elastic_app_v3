using CSharpFunctionalExtensions;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.domain.Errors;
using MediatR;

namespace elastic_app_v3.application.Commands;
public record UpdateProfileCommand(
    Maybe<string> Bio,
    Maybe<IReadOnlyList<LanguageModel>> Languages,
    Maybe<IReadOnlyList<string>> Hobbies,
    Guid UserId
): IRequest<Result<UpdateProfileResponse, ProfileError>>;
