using elastic_app_v3.application.Constants;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.Errors.Identity;
using FluentValidation;

namespace elastic_app_v3.application.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleForUserName();
            RuleForPassword();
        }
        public void RuleForUserName()
        {
            RuleFor(sur => sur.UserName)
                .NotEmpty()
                .WithMessage(IdentityErrorMessages.UserNameEmpty)
                .MinimumLength(ValidationConstants.UserNameMinLength)
                .WithMessage(IdentityErrorMessages.UserNameTooShortMessage())
                .MaximumLength(ValidationConstants.UserNameMaxLength)
                .WithMessage(IdentityErrorMessages.UserNameTooLongMessage());
        }
        public void RuleForPassword()
        {
            RuleFor(sur => sur.Password)
                .NotEmpty()
                .WithMessage(IdentityErrorMessages.PasswordEmpty)
                .MinimumLength(8)
                .WithMessage(IdentityErrorMessages.PasswordTooShortMessage())
                .MaximumLength(22)
                .WithMessage(IdentityErrorMessages.PasswordTooLongMessage()); 
        }
    }
}
