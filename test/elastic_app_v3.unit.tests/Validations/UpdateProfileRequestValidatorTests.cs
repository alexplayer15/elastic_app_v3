using AutoFixture;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Errors.Profile;
using elastic_app_v3.application.Validations;
using FluentValidation.TestHelper;

namespace elastic_app_v3.unit.tests.Validations;
public class UpdateProfileRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateProfileRequestValidator _updateProfileRequestValidator = new();
    public UpdateProfileRequestValidatorTests()
    {
        var languages = new List<LanguageDto>()
        {
            new("English", "Native")
        };

        _fixture.Customize<UpdateProfileRequest>(r => r
            .With(r => r.Bio, "Hello")
            .With(r => r.Languages, languages)
        );
    }

    //[Fact]
    //public void GivenUpdateProfileRequestWithAllNullFields_WhenValidate_ThenReturnValidationError()
    //{
    //    //Arrange 
    //    var request = _fixture.Create<UpdateProfileRequest>() with
    //    {
    //        Bio = null,
    //        Languages = null
    //    };

    //    var result = _updateProfileRequestValidator.TestValidate(request);

    //    result.ShouldHaveValidationErrorFor(r => r)
    //        .WithErrorMessage(ProfileErrorMessages.BlankUpdateProfileRequest);
    //}
}
