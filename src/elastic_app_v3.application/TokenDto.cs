namespace elastic_app_v3.application;

public sealed record TokenDto(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInMinutes
);