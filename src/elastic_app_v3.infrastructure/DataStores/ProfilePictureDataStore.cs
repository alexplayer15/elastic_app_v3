using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.DTOs;
using elastic_app_v3.infrastructure.Config;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.infrastructure.DataStores;
public class ProfilePictureDataStore(
    IAmazonS3 s3Client,
    IOptions<ProfilePictureDataStoreOptions> profilePictureDataStoreOptions,
    ILogger<ProfilePictureDataStore> logger
) : IProfilePictureDataStore
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly ILogger<ProfilePictureDataStore> _logger = logger;
    private readonly ProfilePictureDataStoreOptions _profilePictureDataStoreOptions = profilePictureDataStoreOptions.Value; 
    public Result<ProfilePictureUrls, ProfileError> GetProfilePictureUrls(Guid userId)
    {
        string preSignedUrl = string.Empty;
        var objectKey = $"profile-pictures/{userId}/avatar.jpg";
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _profilePictureDataStoreOptions.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(_profilePictureDataStoreOptions.PreSignedUrlExpirationMinutes),
                Protocol = _profilePictureDataStoreOptions.Protocol
            };
            preSignedUrl = _s3Client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error calling S3");
        }
        catch (AmazonClientException ex)
        {
            _logger.LogError(ex, "Error with AWS Credentials");
        }
        
        var objectUrl = string.IsNullOrEmpty(_profilePictureDataStoreOptions.ObjectUrlBase)
            ? $"https://{_profilePictureDataStoreOptions.BucketName}.s3.{_profilePictureDataStoreOptions.Region}.amazonaws.com/{objectKey}"
            : $"{_profilePictureDataStoreOptions.ObjectUrlBase}/{_profilePictureDataStoreOptions.BucketName}/{objectKey}";
    
        return new ProfilePictureUrls(preSignedUrl, objectUrl);
    }
    
}