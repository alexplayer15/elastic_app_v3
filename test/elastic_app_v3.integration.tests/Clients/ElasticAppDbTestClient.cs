using Dapper;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace elastic_app_v3.integration.tests.Clients;
public class ElasticAppDbTestClient
{
    private readonly string _connectionString;
    private readonly ElasticDatabaseSettings _elasticDatabaseSettings;
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    public ElasticAppDbTestClient()
    {
        _elasticDatabaseSettings = SetElasticDatabaseTestSettings();
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
    private User AddHashedPasswordToTestUser(User user, string password)
    {
        var hashedPassword = _passwordHasher.HashPassword(user, password);

        user.SetPasswordHash(hashedPassword);

        return user;
    }
    private static ElasticDatabaseSettings SetElasticDatabaseTestSettings() => new()
    {
        Database = "Elastic",
        Server = "localhost",
        Port = 1433,
        User = "SA",
        Password = "DonutOfChocolate150!",
        TrustServerCertificate = true
    };
}
