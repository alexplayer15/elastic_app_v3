namespace elastic_app_v3.application.DTOs.Profile;
public sealed record UpdateProfileRequest(
    string? Bio,
    List<LanguageDto>? Languages
);
