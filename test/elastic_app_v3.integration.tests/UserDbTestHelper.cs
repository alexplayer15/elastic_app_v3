using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Constants;
using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.integration.tests
{
    public class UserDbTestHelper
    {
        public UserSettings UserSettings { get; private set; }
        private readonly string _connectionString;
        private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        public UserDbTestHelper()
        {
            UserSettings = SetUserSetting();
            _connectionString = UserSettings.GetConnectionString();
        }
        public async Task AddTestUserAsync(User user, string password)
        {
            var userWithHashedPassword = AddHashedPasswordToTestUser(user, password);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await connection.ExecuteScalarAsync<Guid>(SqlConstants.InsertUser, userWithHashedPassword);
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
                await connection.ExecuteScalarAsync<Guid>(SqlConstants.DeleteUser, new { UserName = userName });
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

        //to do: find a better way to do this
        private static UserSettings SetUserSetting() => new()
        {
            Database = "Users",
            Server = "localhost",
            Port = 1433,
            User = "SA",
            Password = "DonutOfChocolate150!",
            TrustServerCertificate = true
        };
    }
}
