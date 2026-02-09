using elastic_app_v3.Constants;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using FluentValidation;

namespace elastic_app_v3.Validations
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
