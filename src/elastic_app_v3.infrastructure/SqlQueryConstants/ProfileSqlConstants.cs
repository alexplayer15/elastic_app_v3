namespace elastic_app_v3.infrastructure.SqlQueryConstants;
public static class ProfileSqlConstants
{
    public const string AddProfile = @"
    INSERT INTO Profiles (UserId)
    VALUES (@UserId);";

    public const string UpdateBio = @"
    UPDATE Profiles
    SET Bio = CASE WHEN @Bio IS NULL THEN Bio ELSE @Bio END
    OUTPUT inserted.Bio
    WHERE UserId = @UserId;";

    public const string DeleteProfileLanguages = @"
    DELETE FROM Languages
    WHERE UserId = @UserId;";

    public const string AddProfileLanguages = @"
    INSERT INTO Languages (UserId, Type, Proficiency)
    OUTPUT inserted.Type, inserted.Proficiency
    VALUES (@UserId, @Type, @Proficiency);";
    
    public const string UpdateProfilePicture = @"
    UPDATE Profiles
    SET ProfilePictureUrl = @ProfilePictureUrl
    WHERE UserId = @UserId;";
}
