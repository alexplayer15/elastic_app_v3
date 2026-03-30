using System.Text.RegularExpressions;
using elastic_app_v3.infrastructure;

namespace elastic_app_v3.unit.tests;
public partial class SqlUpdateBuilderTests
{

    [Fact]
    public void GivenNonNullColumn_WhenBuild_ThenSqlContainsColumn()
    {
        //Arrange
        var builder = new SqlUpdateBuilder("Profiles");
        builder.SetIfNotNull("Bio", "Hello");
        builder.Where("UserId = @UserId");

        //Act
        var sqlQuery = builder.Build();

        //Assert
        Assert.Contains("UPDATE Profiles", sqlQuery);
        Assert.Contains("SET Bio = @Bio", sqlQuery);
        Assert.Contains("WHERE UserId = @UserId", sqlQuery);
    }
}
