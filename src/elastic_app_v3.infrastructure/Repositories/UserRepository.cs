using System.Net;
using Dapper;
using elastic_app_v3.application.Errors;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Constants;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace elastic_app_v3.infrastructure.Repositories
{
    public class UserRepository(
        IOptions<UserSettings> userSettings,
        ResiliencePipelineProvider<string> resiliencePipelineProvider
    ) : IUserRepository
    {
        private readonly string _connectionString = userSettings.Value.GetConnectionString();
        private readonly ResiliencePipeline _resiliencePipeline 
            = resiliencePipelineProvider.GetPipeline(UserResiliencePolicy.UserResiliencePolicyKey);
        public async Task<Result<Guid>> AddAsync(User user)
        {
            try
            {
                var userName = user.UserName;
                var userExists = await CheckIfUserNameExistsAsync(userName);

                if (userExists)
                {
                    return Result.Fail(new UserAlreadyExistsError());
                }

                Guid userId;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    userId = await connection.ExecuteScalarAsync<Guid>(UserSqlConstants.InsertUser, user);
                }

                return Result.Ok(userId);
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
        }
        public async Task<Result<User>> GetUserByUsernameAsync(string userName)
        {
            User? user;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    user = await connection.QuerySingleOrDefaultAsync<User>(
                        UserSqlConstants.GetUserByUserName,
                        new { UserName = userName }
                    );
                };
                if (user == null)
                {
                    return Result.Fail(new UserDoesNotExistError());
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

            return Result.Ok(user);
        }
        public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
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
                            cancellationToken: token);

                        return await connection.QuerySingleOrDefaultAsync<User>(command);
                    },
                    cancellationToken);
                if (user == null)
                {
                    return Result.Fail(new UserDoesNotExistError());
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

            return Result.Ok(user);
        }
        private async Task<bool> CheckIfUserNameExistsAsync(string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteScalarAsync<bool>(UserSqlConstants.CheckIfUserExists, new { UserName = userName });
        }
    }
}
