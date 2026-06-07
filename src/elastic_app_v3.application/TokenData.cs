namespace elastic_app_v3.application;

public sealed record TokenData(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInMinutes
);