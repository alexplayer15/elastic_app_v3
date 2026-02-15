using Dapper;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Errors;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Constants;
using elastic_app_v3.infrastructure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using elastic_app_v3.domain.Result;

namespace elastic_app_v3.infrastructure.Repositories
{
    public class UserRepository(IOptions<UserSettings> userSettings) : IUserRepository
    {
        private readonly string _connectionString = userSettings.Value.GetConnectionString();
        public async Task<Result<Guid>> AddAsync(User user)
        {
            try
            {
                var userName = user.UserName;
                var userExists = await CheckIfUserNameExistsAsync(userName);

                if (userExists)
                {
                    return Result<Guid>.Failure(UserErrors.UserAlreadyExistsError(userName));
                }

                Guid userId;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    userId = await connection.ExecuteScalarAsync<Guid>(SqlConstants.InsertUser, user);
                }

                return Result<Guid>.Success(userId);
            }
            catch (SqlException ex)
            {
                return Result<Guid>.Failure(DataBaseErrors.SqlDatabaseError(ex.Message));
            }
            catch (TimeoutException ex)
            {
                return Result<Guid>.Failure(DataBaseErrors.SqlTimeoutError(ex.Message));
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
                        SqlConstants.GetUserByUserName,
                        new { UserName = userName }
                    );
                };
                if (user == null)
                {
                    return Result<User>.Failure(UserErrors.UserDoesNotExistError);
                }
            }
            catch (SqlException ex)
            {
                return Result<User>.Failure(DataBaseErrors.SqlDatabaseError(ex.Message));
            }
            catch (TimeoutException ex)
            {
                return Result<User>.Failure(DataBaseErrors.SqlTimeoutError(ex.Message));
            }
            catch (Exception)
            {
                throw;
            }

            return Result<User>.Success(user);
        }
        public async Task<Result<User>> GetUserByIdAsync(Guid userId)
        {
            User? user;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    user = await connection.QuerySingleOrDefaultAsync<User>(
                        SqlConstants.GetUserById,
                        new { UserId = userId }
                    );
                };
                if (user == null)
                {
                    return Result<User>.Failure(UserErrors.UserDoesNotExistError);
                }
            }
            catch (SqlException ex)
            {
                return Result<User>.Failure(DataBaseErrors.SqlDatabaseError(ex.Message));
            }
            catch (TimeoutException ex)
            {
                return Result<User>.Failure(DataBaseErrors.SqlTimeoutError(ex.Message));
            }
            catch (Exception)
            {
                throw;
            }

            return Result<User>.Success(user);
        }
        private async Task<bool> CheckIfUserNameExistsAsync(string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteScalarAsync<bool>(SqlConstants.CheckIfUserExists, new { UserName = userName });
        }
    }
}
