namespace elastic_app_v3.application.DTOs.Login;
public sealed record LoginRequest(
    string UserName,
    string Password
);
