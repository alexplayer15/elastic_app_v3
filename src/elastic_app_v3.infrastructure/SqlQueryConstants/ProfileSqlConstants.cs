namespace elastic_app_v3.infrastructure.SqlQueryConstants;
public static class ProfileSqlConstants
{
    public const string AddProfile = @"
    INSERT INTO Profiles (UserId)
    VALUES (@UserId);";

    public const string GetProfileByUserId = @"
    SELECT UserId, Bio
    FROM Profiles
    WHERE UserId = @UserId;";

    public const string UpdateProfile = @"
    UPDATE Profiles
    SET Bio = @Bio
    OUTPUT inserted.Bio
    WHERE UserId = @UserId;";

    public const string DeleteProfileLanguages = @"
    DELETE FROM Languages
    WHERE UserId = @UserId;";

    public const string AddProfileLanguages = @"
    INSERT INTO Languages (UserId, Type, Proficiency)
    OUTPUT inserted.Type, inserted.Proficiency
    VALUES (@UserId, @Type, @Proficiency);";
}
