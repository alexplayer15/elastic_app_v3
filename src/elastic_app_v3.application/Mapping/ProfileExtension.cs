using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.ValueObjectExtensions;
using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.application.Mapping
{
    public static class ProfileExtension
    {
        public static UpdateProfileResponse ToDto(this Profile profile)
        {
            return new UpdateProfileResponse(
                profile.Bio,
                profile.Languages?.Select(l => l.ToDto()).ToList() ?? []
            );
        }
    }
}
