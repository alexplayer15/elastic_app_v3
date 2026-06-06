using System.Net;
using elastic_app_v3.common.tests;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.e2e.tests.Constants;
using elastic_app_v3.e2e.tests.Fixtures;

namespace elastic_app_v3.e2e.tests.Tests.GetProfilePictureUrl;

public class GetProfilePictureUrlTests
{
    [Collection(TestCollectionConstants.EndToEndTestCollectionName)]
    public class GetUserByIdTests(EndToEndTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);

        // public async Task Given_When_GetProfilePictureUrl_ThenReturnPreSignedUrl()
        // {
        //     //Arrange
        //     var token = TokenHelper.GenerateTestToken(Guid.NewGuid());
        //     
        //     //Act
        //     var response = await _apiClient.SendGetProfilePictureUrlRequest(token);
        //     
        //     //Assert
        //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        // }
    }
}