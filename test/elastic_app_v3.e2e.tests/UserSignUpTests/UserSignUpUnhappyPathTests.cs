using System.Net;
using AutoFixture;
using elastic_app_v3.api.Errors;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.e2e.tests.InfraHelpers;
using elastic_app_v3.e2e.tests.SetUp;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.e2e.tests.UserSignUpTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class UserSignUpUnhappyPathTests
    {
        private readonly ApiClient _apiClient;
        private readonly Fixture _fixture = new();
        private readonly UserDbTestHelper _userDbTestHelper = new();
        public UserSignUpUnhappyPathTests(IntegrationTestFixture fixture)
        {
            _apiClient = new ApiClient(fixture.Client);

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
        }

        [Fact]
        public async Task GivenUserAlreadyExists_WhenSendUserSignUpRequest_ThenReturn409AndUserExistsError()
        {
            //Arrange
            var requestBody = _fixture.Create<SignUpRequest>();

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, requestBody.FirstName)
                .With(u => u.LastName, requestBody.LastName)
                .With(u => u.UserName, requestBody.UserName)
                .With(u => u.PasswordHash, requestBody.Password)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user, requestBody.Password);

            //Act
            var httpResponse = await _apiClient.SendUserSignupRequest(requestBody);

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, httpResponse.StatusCode);
            var error = await _apiClient.GetResponseAsync<ProblemDetails>(httpResponse);
            Assert.NotNull(error);
            Assert.Equal(ErrorCodes.UserAlreadyExistsError, error.Type);
        }

        [Fact]
        public async Task GivenInValidUserSignUpRequest_WhenSendUserSignUpRequest_ThenReturn400AndValidationError()
        {
            //Arrange
            var requestBody = _fixture.Create<SignUpRequest>() with
            {
                FirstName = string.Empty
            };

            ///Act
            var httpResponse = await _apiClient.SendUserSignupRequest(requestBody);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            var error = await _apiClient.GetResponseAsync<ProblemDetails>(httpResponse);
            Assert.NotNull(error);
            Assert.Equal(ErrorCodes.ValidationError, error.Type);
        }
    }
}
