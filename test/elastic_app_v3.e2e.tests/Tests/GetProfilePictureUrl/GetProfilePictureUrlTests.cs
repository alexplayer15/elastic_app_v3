using System.Net;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.common.tests;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.e2e.tests.Constants;
using elastic_app_v3.e2e.tests.Fixtures;

namespace elastic_app_v3.e2e.tests.Tests.GetProfilePictureUrl;

[Collection(TestCollectionConstants.EndToEndTestCollectionName)]
public class GetProfilePictureUrlTests(EndToEndTestFixture fixture)
{
    private readonly ApiClient _apiClient = new(fixture.Client);
    
    [Fact]
    public async Task GivenValidUser_WhenSendGetProfilePictureUrlRequest_GetProfilePictureUrl_ThenReturnPreSignedUrl()
    {
        //Arrange
        var token = TokenHelper.GenerateTestToken(Guid.NewGuid());
        
        //Act
        var response = await _apiClient.SendGetProfilePictureUrlRequest(token);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var getProfilePictureUrlResponse = await _apiClient.GetResponseAsync<GetProfilePictureUrlResponse>(response);
        Assert.NotNull(getProfilePictureUrlResponse);
        Assert.NotEmpty(getProfilePictureUrlResponse.PreSignedUrl);
        Assert.NotEmpty(getProfilePictureUrlResponse.ObjectUrl);
    }
}
