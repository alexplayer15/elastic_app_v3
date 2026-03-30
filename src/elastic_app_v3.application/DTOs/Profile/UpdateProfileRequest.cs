using FluentResults;
using MediatR;

namespace elastic_app_v3.application.DTOs.Profile;
public sealed record UpdateProfileRequest(
    string? Bio, 
    List<LanguageDto>? Languages
) : IRequest<Result<UpdateProfileResponse>>
{
    public Guid UserId { get; set; } //this sucks, not sure how to get around it
}
