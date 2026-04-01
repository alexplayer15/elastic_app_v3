using elastic_app_v3.application.DTOs.Profile;
using FluentResults;
using MediatR;

namespace elastic_app_v3.application.Commands;
public sealed record UpdateProfileCommand(
    string? Bio,
    List<LanguageModel>? Languages,
    Guid UserId
) : IRequest<Result<UpdateProfileResponse>>;
