namespace elastic_app_v3.infrastructure.Constants
{
    public static class SqlConstants
    {
        public const string GetUserById = @"
        SELECT Id, Firstname, Lastname, Username, PasswordHash, CreatedAt
        FROM Users
        WHERE Id = @UserId;";

        public const string GetUserId = @"
        SELECT Id
        FROM Users
        WHERE Username = @Username;";

        public const string InsertUser = @"
        INSERT INTO Users (FirstName, LastName, UserName, PasswordHash)
        OUTPUT INSERTED.Id
        VALUES (@FirstName, @LastName, @UserName, @PasswordHash);";

        public const string DeleteUser = @"
        DELETE FROM Users
        WHERE Username = @Username;";

        public const string CheckIfUserExists = @"
        SELECT CAST(
            CASE WHEN EXISTS (SELECT 1 FROM Users WHERE UserName = @UserName)
            THEN 1 ELSE 0 END AS BIT
        );";

        public const string GetUserByUserName = @"
        SELECT Id, Firstname, Lastname, UserName, PasswordHash
        FROM Users
        WHERE UserName = @UserName;";
    }
}
