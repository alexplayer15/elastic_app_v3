using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Dapper;
using elastic_app_v3.common.tests.Config;

namespace elastic_app_v3.common.tests.Clients;
public class ElasticAppDbClient
{
    private readonly ElasticDatabaseSettings _elasticDatabaseSettings;
    private readonly string _connectionString;
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    public ElasticAppDbClient()
    {
        _elasticDatabaseSettings = ElasticAppDbTestSettings.SetElasticDatabaseTestSettings();
        _connectionString = _elasticDatabaseSettings.GetConnectionString();
    }
    public async Task AddTestUserAsync(User user, string password)
    {
        var userWithHashedPassword = AddHashedPasswordToTestUser(user, password);

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var userId = await connection.ExecuteScalarAsync<Guid>(UserSqlConstants.AddUser, userWithHashedPassword);
            await connection.ExecuteAsync(ProfileSqlConstants.AddProfile, new { UserId = userId });
        }
        catch (Exception ex)
        {
            throw new Exception("Something went wrong adding test user", ex);
        }
    }
    public async Task<Guid> AddTestUserAsync(User user)
    {
        var userWithHashedPassword = AddHashedPasswordToTestUser(user, "password");

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var userId = await connection.ExecuteScalarAsync<Guid>(UserSqlConstants.AddUser, userWithHashedPassword);
            await connection.ExecuteAsync(ProfileSqlConstants.AddProfile, new { UserId = userId });

            return userId;
        }
        catch (Exception ex)
        {
            throw new Exception("Something went wrong adding test user", ex);
        }
    }
    public async Task DeleteTestUserAsync(string userName)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteScalarAsync<Guid>(UserSqlConstants.DeleteUser, new { UserName = userName });
        }
        catch (Exception ex)
        {
            throw new Exception("Something went wrong deleting test user", ex);
        }
    }
    private User AddHashedPasswordToTestUser(User user, string password)
    {
        var hashedPassword = _passwordHasher.HashPassword(user, password);

        user.SetPasswordHash(hashedPassword);

        return user;
    }
}

