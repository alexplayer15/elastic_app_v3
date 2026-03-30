namespace elastic_app_v3.application.DTOs.Profile;
public sealed record UpdateProfileResponse(string? Bio, List<LanguageDto> Languages);
