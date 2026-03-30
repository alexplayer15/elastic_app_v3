using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.application.ValueObjectExtensions
{
    public static class LanguageExtension
    {
        public static LanguageDto ToDto(this Language languageEntity)
        {
            return new LanguageDto(
                languageEntity.Type,
                languageEntity.Proficiency
            );
        }
    }
}
