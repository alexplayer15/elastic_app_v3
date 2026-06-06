namespace elastic_app_v3.infrastructure.Config;

public class ProfilePictureDataStoreOptions
{
    public const string ProfilePictureDataStoreOptionsName = "ProfilePictureDataStore";
    public string BucketName { get; init; } = string.Empty;
    public int PreSignedUrlExpirationMinutes { get; init; } = 15;
}