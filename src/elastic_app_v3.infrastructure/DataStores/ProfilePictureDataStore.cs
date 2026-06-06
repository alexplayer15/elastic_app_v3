using Amazon.S3;
using Amazon.S3.Model;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Models;
using elastic_app_v3.infrastructure.Config;
using FluentResults;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.infrastructure.DataStores;
public class ProfilePictureDataStore(
    IAmazonS3 s3Client,
    IOptions<ProfilePictureDataStoreOptions> profilePictureDataStoreOptions
) : IProfilePictureDataStore
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly ProfilePictureDataStoreOptions _profilePictureDataStoreOptions = profilePictureDataStoreOptions.Value; 
    public Result<ProfilePictureUrls> GetProfilePictureUrls(Guid userId)
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
            };
            preSignedUrl = _s3Client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception ex)
        {
            //to do: add logging
        }
        
        var objectUrl = $"https://{_profilePictureDataStoreOptions.BucketName}.s3.{_profilePictureDataStoreOptions.Region}.amazonaws.com/{objectKey}";
    
        return new ProfilePictureUrls(preSignedUrl, objectUrl);
    }
    
}