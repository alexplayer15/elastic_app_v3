using Amazon.S3;

namespace elastic_app_v3.infrastructure.Config;

public class ProfilePictureDataStoreOptions
{
    public const string ProfilePictureDataStoreOptionsName = "ProfilePictureDataStore";
    public string BucketName { get; init; } = string.Empty;
    public int PreSignedUrlExpirationMinutes { get; init; } = 15;
    public string Region { get; init; } = string.Empty;
    public string ServiceUrl { get; init; } = string.Empty;
    public bool IsLocal { get; init; } = false;
    public Protocol Protocol { get; init; } = Protocol.HTTPS;
    public string? ObjectUrlBase { get; init; } = null; //only for local dev - to provide object url browser can find (not a fan of this but no better solution yet)
}