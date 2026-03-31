using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Errors.Profile;
using FluentValidation;

namespace elastic_app_v3.application.Validations;
public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleForLanguages();
        RuleForBlankRequest();
    }
    public void RuleForLanguages()
    {
        RuleFor(x => x.Languages)
            .NotEmpty()
            .When(x => x.Languages is not null)
            .WithMessage(ProfileErrorMessages.NoLanguagesProvided);
    }
    public void RuleForBlankRequest()
    {
        RuleFor(x => x)
         .Must(x => x.GetType().GetProperties()
                     .Any(p => p.GetValue(x) != null))
         .WithMessage(ProfileErrorMessages.BlankUpdateProfileRequest);
    }
}
