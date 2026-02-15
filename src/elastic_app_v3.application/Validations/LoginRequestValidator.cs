using elastic_app_v3.application.Constants;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Errors;
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
                .WithMessage(ErrorMessages.UserNameEmpty)
                .MinimumLength(ValidationConstants.UserNameMinLength)
                .WithMessage(ErrorMessages.UserNameTooShortMessage())
                .MaximumLength(ValidationConstants.UserNameMaxLength)
                .WithMessage(ErrorMessages.UserNameTooLongMessage());
        }
        public void RuleForPassword()
        {
            RuleFor(sur => sur.Password)
                .NotEmpty()
                .WithMessage(ErrorMessages.PasswordEmpty)
                .MinimumLength(8)
                .WithMessage(ErrorMessages.PasswordTooShortMessage())
                .MaximumLength(22)
                .WithMessage(ErrorMessages.PasswordTooLongMessage()); 
        }
    }
}
