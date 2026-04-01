using AutoFixture;
using elastic_app_v3.application.Errors.Identity;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Repositories;
using elastic_app_v3.integration.tests.Clients;
using elastic_app_v3.integration.tests.Fixtures;
using Microsoft.Extensions.Options;
using NSubstitute;
using Polly;
using Polly.Registry;

namespace elastic_app_v3.integration.tests.ElasticAppDb.Tests;

[Collection(TestCollectionConstants.ElasticAppDbTestCollectionName)]
public class UserRepositoryTests
{
    private readonly ElasticAppDbTestClient _dbClient = new();
    private readonly IUserRepository _userRepository;
    private readonly Fixture _fixture = new();
    public UserRepositoryTests(ElasticAppDbFixture fixture)
    {
        var dbSettings = Options.Create(fixture.ElasticDatabaseSettings);

        var pipeline = new ResiliencePipelineBuilder()
        .Build(); 

        var provider = Substitute.For<ResiliencePipelineProvider<string>>();

        provider.GetPipeline(Arg.Any<string>())
                .Returns(pipeline);

        _userRepository = new UserRepository(dbSettings, provider);
    }

    [Fact]
    public async Task GivenSignedUpUser_WhenGetUserByUsername_ThenReturnUser()
    {
        //Arrange
        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];
        var expectedUser = _fixture.Build<User>()
            .With(u => u.FirstName, "Leon")
            .With(u => u.LastName, "Kennedy")
            .With(u => u.UserName, username)
            .Without(u => u.PasswordHash)
            .Create();

        await _dbClient.AddTestUserAsync(expectedUser, "password"); //need password input?

        //Act
        var userResult = await _userRepository.GetUserByUsernameAsync(username, CancellationToken.None);

        //Assert
        Assert.True(userResult.IsSuccess);
        Assert.NotNull(userResult.Value);
        Assert.NotEqual(Guid.Empty, userResult.Value.Id);
        Assert.Equal(expectedUser.FirstName, userResult.Value.FirstName);
        Assert.Equal(expectedUser.LastName, userResult.Value.LastName);
        Assert.Equal(expectedUser.UserName, userResult.Value.UserName);
    }

    [Fact]
    public async Task GivenUserDoesNotExist_WhenGetUserByUsername_ThenReturnUserDoesNotExistError()
    {
        //Arrange
        var maxUsernameLength = 22;
        //GUID length with N is 32 chars
        var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

        //Act
        var userResult = await _userRepository.GetUserByUsernameAsync(username, CancellationToken.None);

        //Assert
        Assert.True(userResult.IsFailed);
        Assert.Single(userResult.Errors);
        Assert.IsType<UserDoesNotExistError>(userResult.Errors[0]);
    }
}
