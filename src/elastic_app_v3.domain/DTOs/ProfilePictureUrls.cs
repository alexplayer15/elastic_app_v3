namespace elastic_app_v3.domain.DTOs;

public sealed record ProfilePictureUrls(
    string PreSignedUrl, 
    string  ObjectUrl
);