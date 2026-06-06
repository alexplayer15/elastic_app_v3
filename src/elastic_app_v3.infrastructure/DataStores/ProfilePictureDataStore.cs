using Amazon.S3;
using Amazon.S3.Model;
using elastic_app_v3.domain.Abstractions;
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
    public Result<string> GetProfilePictureUrl(Guid userId)
    {
        string urlString = string.Empty;
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _profilePictureDataStoreOptions.BucketName,
                Key = $"profile-pictures/{userId}/avatar.jpg",
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(_profilePictureDataStoreOptions.PreSignedUrlExpirationMinutes),
            };
            urlString = _s3Client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception ex)
        {
            throw; //to do: come back and add meaningful error handling 
        }
    
        return urlString;
    }
    
}