using AutoFixture;
using System.Net;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.integration.tests.Clients;
using elastic_app_v3.integration.tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.api.Errors;
using elastic_app_v3.application.DTOs.Login;

namespace elastic_app_v3.integration.tests.Api.Tests;

[Collection(TestCollectionConstants.ApiTestCollectionName)]
public class ValidationTests
{
    private readonly ApiClient _apiClient;
    private readonly Fixture _fixture = new();
    public ValidationTests(ApiFixture fixture)
    {
        _apiClient = new(fixture.Client);

        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

        _fixture.Customize<SignUpRequest>(c => c
            .With(x => x.FirstName, "Alex")
            .With(x => x.LastName, "Player")
            .With(x => x.UserName, username)
            .With(x => x.Password, "password") 
            .With(x => x.ReEnteredPassword, "password")
        );

        _fixture.Customize<LoginRequest>(c => c
            .With(x => x.UserName, username)
            .With(x => x.Password, "password")
        );
    }

    [Fact]
    public async Task GivenInvalidSignUpRequest_WhenSendSignUpRequest_ThenReturn400AndValidationError()
    {
        //Arrange
        var request = _fixture.Create<SignUpRequest>() with
        {
            FirstName = null!
        };

        //Act
        var response = await _apiClient.SendUserSignupRequest(request);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await _apiClient.GetResponseAsync<ProblemDetails>(response);
        Assert.NotNull(error);
        Assert.Equal(ErrorCodes.ValidationError, error.Type);
    }

    [Fact]
    public async Task GivenInvalidLoginRequest_WhenSendLoginRequest_ThenReturn400AndValidationError()
    {
        //Arrange
        var request = _fixture.Create<LoginRequest>() with
        {
            UserName = null!
        };

        //Act
        var response = await _apiClient.SendUserLoginRequest(request);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await _apiClient.GetResponseAsync<ProblemDetails>(response);
        Assert.NotNull(error);
        Assert.Equal(ErrorCodes.ValidationError, error.Type);
    }
}
