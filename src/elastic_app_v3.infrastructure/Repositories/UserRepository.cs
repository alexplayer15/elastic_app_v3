using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Identity;
using elastic_app_v3.domain.Events;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace elastic_app_v3.infrastructure.Repositories
{
    public class UserRepository(
        IOptions<ElasticDatabaseSettings> elasticDatabaseSettings,
        ResiliencePipelineProvider<string> resiliencePipelineProvider
    ) : IUserRepository
    {
        private readonly string _connectionString = elasticDatabaseSettings.Value.GetConnectionString();
        private readonly ResiliencePipeline _resiliencePipeline 
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.ElasticAppDatabaseResiliencePolicyKey);
        public async Task<UnitResult<UserError>> AddAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var userName = user.UserName;
                var userExists = await CheckIfUserNameExistsAsync(userName, cancellationToken);

                if (userExists)
                {
                    return UnitResult.Failure<UserError>(new UserAlreadyExistsError());
                }

                await _resiliencePipeline.ExecuteAsync(
                    async token =>
                    {
                        using var connection = new SqlConnection(_connectionString);
                        await connection.OpenAsync(token);

                        using var transaction = connection.BeginTransaction();

                        var addUserCommand = new CommandDefinition(
                            UserSqlConstants.AddUser,
                            user,
                            transaction,
                            cancellationToken: token
                        );

                        var userId = await connection.ExecuteScalarAsync<Guid>(addUserCommand); //will this continue if the command returns null? When would it return null?

                        var addProfileCommand = new CommandDefinition(
                            ProfileSqlConstants.AddProfile,
                            new { UserId = userId },
                            transaction,
                            cancellationToken: token
                        );

                        await connection.ExecuteAsync(addProfileCommand);

                        var userSignedUpEvent = new UserSignedUpEvent(userId);

                        var outboxCommand = new CommandDefinition(
                            OutboxEventSqlConstants.AddUserSignedUpEvent,
                            new
                            {
                                Type = nameof(UserSignedUpEvent),
                                Payload = JsonSerializer.Serialize(userSignedUpEvent)
                            },
                            transaction: transaction,
                            cancellationToken: token
                        );

                        await connection.ExecuteAsync(outboxCommand);

                        await transaction.CommitAsync(token);

                    }, cancellationToken);
   
            }
            catch (SqlException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw; //to do: think how best to handle this;
            }
            catch (Exception)
            {
                throw;
            }

            return UnitResult.Success<UserError>();
        }
        public async Task<Result<User, UserError>> GetUserByUsernameAsync(
            string userName, 
            CancellationToken cancellationToken
        )
        {
            var user = await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    await using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);

                    return await connection.QuerySingleOrDefaultAsync<User>(
                        UserSqlConstants.GetUserByUserName,
                        new { UserName = userName });
                }, cancellationToken
            );

            if (user is null)
            {
                return Result.Failure<User, UserError>(new UserDoesNotExistError());
            }

            //later could add try/catch with logging and domain exceptions/errors

            return Result.Success<User, UserError>(user);
        }
        public async Task<Result<User, UserError>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            User? user;
            try
            {
                user = await _resiliencePipeline.ExecuteAsync(
                    async token =>
                    {
                        await using var connection = new SqlConnection(_connectionString);

                        await connection.OpenAsync(token);

                        var command = new CommandDefinition(
                            UserSqlConstants.GetUserById,
                            new { UserId = userId },
                            cancellationToken: token
                        );

                        return await connection.QuerySingleOrDefaultAsync<User>(command);
                    },
                    cancellationToken);
                if (user == null)
                {
                    return Result.Failure<User, UserError>(new UserDoesNotExistError());
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw; //to do: think how best to handle this;
            }
            catch (Exception)
            {
                throw;
            }

            return Result.Success<User, UserError>(user);
        }
        private async Task<bool> CheckIfUserNameExistsAsync(string userName, CancellationToken cancellationToken)
        {
            return await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                     await using var connection = new SqlConnection(_connectionString);
                     await connection.OpenAsync(token);

                    var command = new CommandDefinition(
                        UserSqlConstants.CheckIfUserExists,
                        new { UserName = userName },
                        cancellationToken: token
                    );
                    return await connection.ExecuteScalarAsync<bool>(command);
                },
                cancellationToken);
        }
    }
}
