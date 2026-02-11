using elastic_app_v3.Config;
using elastic_app_v3.Constants;
using elastic_app_v3.Domain;
using Microsoft.Data.SqlClient;
using Dapper;

namespace elastic_app_v3.integration.tests
{
    public class UserDbTestHelper
    {
        public UserSettings UserSettings { get; private set; }
        private readonly string _connectionString;
        public UserDbTestHelper()
        {
            UserSettings = SetUserSetting();
            _connectionString = UserSettings.GetConnectionString();
        }
        public async Task AddTestUserAsync(User user)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await connection.ExecuteScalarAsync<Guid>(SqlConstants.InsertUser, user);
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
