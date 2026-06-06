namespace elastic_app_v3.application.DTOs.Profile;

public sealed record GetProfilePictureUrlResponse(
    string PreSignedUrl,
    string ObjectUrl
);