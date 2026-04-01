using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.domain.Models;
using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.application.Mapping;
public static class UpdateProfileDtoToDomainModelMapper
{
    public static ProfileUpdate ToDomainModel(this UpdateProfileRequest request)
    {
        return new ProfileUpdate
        {
            UserId = request.UserId,
            Bio = request.Bio ?? string.Empty,
            Languages = request.Languages is null ? null : [.. request.Languages.Select(l => new Language(l.Type, l.Proficiency))]
        };
    }
}
