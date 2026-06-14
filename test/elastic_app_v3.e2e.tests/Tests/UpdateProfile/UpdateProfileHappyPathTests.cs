using System.Net;
using AutoFixture;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.common.tests;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.e2e.tests.Constants;
using elastic_app_v3.e2e.tests.Fixtures;

namespace elastic_app_v3.e2e.tests.Tests.UpdateProfile;

[Collection(TestCollectionConstants.EndToEndTestCollectionName)]
public class UpdateProfileHappyPathTests
{
    private readonly ApiClient _apiClient;
    private readonly Fixture _fixture = new();
    private readonly ElasticAppDbClient _userDbTestHelper = new();
    public UpdateProfileHappyPathTests(EndToEndTestFixture fixture)
    {
        _apiClient = new(fixture.Client);

        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

        _fixture.Customize<User>(c => c
              .With(u => u.FirstName, "Alex")
              .With(u => u.LastName, "Player")
              .With(u => u.UserName, username)
              .Without(u => u.PasswordHash)
        );
    }

    [Theory]
    [MemberData(nameof(UpdateProfileTestCases))]
    public async Task GivenLoggedInUser_WhenSendUpdateProfileRequest_ThenReturnUpdatedProfile(
        string? bio, 
        List<LanguageDto>? languages,
        List<string>? hobbies)
    {
        //Arrange
        var user = _fixture.Create<User>();
        var userId = await _userDbTestHelper.AddTestUserAsync(user);

        var token = TokenHelper.GenerateTestToken(userId);

        var request = _fixture.Build<UpdateProfileRequest>()
            .With(upr => upr.Bio, bio)
            .With(upr => upr.Languages, languages)
            .With(upr => upr.Hobbies, hobbies)
            .Create();

        //Act
        var response = await _apiClient.SendUpdateProfileRequest(request, token);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updateProfileResponse = await _apiClient.GetResponseAsync<UpdateProfileResponse>(response);
        Assert.NotNull(updateProfileResponse);
        var expectedBio = bio ?? string.Empty;
        Assert.Equal(expectedBio, updateProfileResponse.Bio);
        var expectedLanguages = languages ?? [];
        Assert.Equal(expectedLanguages, updateProfileResponse.Languages);
    }
    public static TheoryData<string?, List<LanguageDto>?, List<string>?> UpdateProfileTestCases =>
    new()
    {
        { "Hello", [new LanguageDto("English", "Native")], ["Football"] },
        { "Hello", null, null },
        { null, [new LanguageDto("English", "Native")], null }
    };
}
