using AutoFixture;
using System.Net;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.integration.tests.SetUp;

namespace elastic_app_v3.integration.tests.UpdateProfileTests;

[Collection(TestCollectionConstants.IntegrationTestCollectionName)]
public class UpdateProfileHappyPathTests(IntegrationTestFixture fixture)
{
    private readonly ApiClient _apiClient = new(fixture.Client);
    private readonly Fixture _fixture = new();

    //TO DO: Find way to do this test w/o having to do the whole journey, very complex + expensive otherwise
    [Fact]
    public async Task GivenLoggedInUser_WhenSendUpdateProfileRequest_ThenReturnUpdatedProfile()
    {
        //Arrange
        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];
        var password = "password";

        //sign up
        var signUpRequest = _fixture.Build<SignUpRequest>()
            .With(x => x.FirstName, "Alex")
            .With(x => x.LastName, "Player")
            .With(x => x.UserName, username)
            .With(x => x.Password, password)
            .With(x => x.ReEnteredPassword, password)
            .Create();

        await _apiClient.SendUserSignupRequest(signUpRequest);

        //login
        var loginRequest = _fixture.Build<LoginRequest>()
               .With(u => u.UserName, username)
               .With(l => l.Password, password)
               .Create();

        var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
        var loginResult = await _apiClient.GetResponseAsync<LoginResponse>(loginResponse);
        var token = loginResult!.AccessToken;

        var userId = Guid.NewGuid();
        var request = _fixture.Build<UpdateProfileRequest>()
            .With(upr => upr.UserId, userId)
            .With(upr => upr.Bio, "Hello")
            .With(upr => upr.Languages, new List<LanguageDto>()
            {
                new("English", "Native")
            })
            .Create();

        //Act
        var response = await _apiClient.SendUpdateProfileRequest(request, token);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    //[Fact]
    //public async Task GivenLoggedInUser_WhenSendUpdateProfileRequestWithPartialUpdates_ThenReturnUpdatedProfile()
    //{
    //    //Arrange
    //    var maxUsernameLength = 22;
    //    //GUID length with N is 32 chars
    //    var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];
    //    var password = "password";

    //    //sign up
    //    var signUpRequest = _fixture.Build<SignUpRequest>()
    //        .With(x => x.FirstName, "Alex")
    //        .With(x => x.LastName, "Player")
    //        .With(x => x.UserName, username)
    //        .With(x => x.Password, password)
    //        .With(x => x.ReEnteredPassword, password)
    //        .Create();

    //    await _apiClient.SendUserSignupRequest(signUpRequest);

    //    //login
    //    var loginRequest = _fixture.Build<LoginRequest>()
    //           .With(u => u.UserName, username)
    //           .With(l => l.Password, password)
    //           .Create();

    //    var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
    //    var loginResult = await _apiClient.GetResponseAsync<LoginResponse>(loginResponse);
    //    var token = loginResult!.AccessToken;

    //    var userId = Guid.NewGuid();
    //    var request = _fixture.Build<UpdateProfileRequest>()
    //        .With(upr => upr.UserId, userId)
    //        .With(upr => upr.Bio, "Hello")
    //        .With(upr => upr.Languages, new List<LanguageDto>()
    //        {
    //            new("English", "Native")
    //        })
    //        .Create();

    //    //Act
    //    var response = await _apiClient.SendUpdateProfileRequest(request, token);

    //    //Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //}
}
