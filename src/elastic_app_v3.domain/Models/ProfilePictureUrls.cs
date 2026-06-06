namespace elastic_app_v3.domain.Models;

public sealed record ProfilePictureUrls(
    string PreSignedUrl, 
    string  ObjectUrl
);