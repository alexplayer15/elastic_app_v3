using System.Net;
using AutoFixture;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.common.tests;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.e2e.tests.Constants;
using elastic_app_v3.e2e.tests.Fixtures;

namespace elastic_app_v3.e2e.tests.Tests.SaveProfilePicture;

[Collection(TestCollectionConstants.EndToEndTestCollectionName)]
public class SaveProfilePictureTests(EndToEndTestFixture fixture)
{
    private readonly ApiClient _apiClient = new(fixture.Client);
    private readonly ElasticAppDbClient _userDbTestHelper = new();
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GivenValidObjectUrl_WhenSendSaveProfilePictureRequest_ThenSaveProfilePicture()
    {
        //Arrange
        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

        var user = _fixture.Build<User>()
            .With(u => u.FirstName, "Alex")
            .With(u => u.LastName, "Player")
            .With(u => u.UserName, username)
            .Without(u => u.PasswordHash)
            .Create();

        var userId = await _userDbTestHelper.AddTestUserAsync(user);

        var token = TokenHelper.GenerateTestToken(userId);
        
        const string objectUrl = $"https://elastic-app-profile-pictures.s3.eu-west-1.amazonaws.com/test-object-key";

        var request = new SaveProfilePictureRequest(objectUrl);
        
        //Act
        var response = await _apiClient.SendSaveProfileRequest(request, token);
        
        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }
}