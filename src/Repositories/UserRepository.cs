using Dapper;
using elastic_app_v3.Config;
using elastic_app_v3.Constants;
using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.Repositories
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

        public async Task<Result<UserSchema>> GetUserByUsernameAsync(LoginRequest loginRequest)
        {
            UserSchema? user;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    user = await connection.QuerySingleOrDefaultAsync<UserSchema>(
                        SqlConstants.GetUserByUserName,
                        new { UserName = loginRequest.UserName }
                    );
                };
                if (user == null)
                {
                    return Result<UserSchema>.Failure(UserErrors.UserDoesNotExistError);
                }
            }
            catch (SqlException ex)
            {
                return Result<UserSchema>.Failure(DataBaseErrors.SqlDatabaseError(ex.Message));
            }
            catch (TimeoutException ex)
            {
                return Result<UserSchema>.Failure(DataBaseErrors.SqlTimeoutError(ex.Message));
            }
            catch (Exception)
            {
                throw;
            }

            return Result<UserSchema>.Success(user);
        }
        public async Task<Result<UserSchema>> GetUserByIdAsync(Guid userId)
        {
            UserSchema? user;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    user = await connection.QuerySingleOrDefaultAsync<UserSchema>(
                        SqlConstants.GetUserById,
                        new { UserId = userId }
                    );
                };
                if (user == null)
                {
                    return Result<UserSchema>.Failure(UserErrors.UserDoesNotExistError);
                }
            }
            catch (SqlException ex)
            {
                return Result<UserSchema>.Failure(DataBaseErrors.SqlDatabaseError(ex.Message));
            }
            catch (TimeoutException ex)
            {
                return Result<UserSchema>.Failure(DataBaseErrors.SqlTimeoutError(ex.Message));
            }
            catch (Exception)
            {
                throw;
            }

            return Result<UserSchema>.Success(user);
        }
        private async Task<bool> CheckIfUserNameExistsAsync(string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteScalarAsync<bool>(SqlConstants.CheckIfUserExists, new { UserName = userName });
        }
    }
}
