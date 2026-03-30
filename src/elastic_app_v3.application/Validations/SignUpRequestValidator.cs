using elastic_app_v3.application.Constants;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.application.Errors.Identity;
using FluentValidation;

namespace elastic_app_v3.application.Validations
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleForFirstName();
            RuleForLastName();
            RuleForUserName();
            RuleForPassword();
            RuleForReEnteredPassword();
        }
        public void RuleForFirstName()
        {
            RuleFor(sur => sur.FirstName)
                .NotEmpty()
                .WithMessage(IdentityErrorMessages.FirstNameEmpty)
                .Matches("^[a-zA-z]+$")
                .WithMessage(IdentityErrorMessages.FirstNameNonAlphabetical);
        }
        public void RuleForLastName()
        {
            RuleFor(sur => sur.LastName)
                .NotEmpty()
                .WithMessage(IdentityErrorMessages.LastNameEmpty)
                .Matches("^[a-zA-z]+$")
                .WithMessage(IdentityErrorMessages.LastNameNonAlphabetical);
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
                .WithMessage(IdentityErrorMessages.PasswordTooLongMessage()); // to do: think of validations for password
        }
        public void RuleForReEnteredPassword()
        {
            RuleFor(sur => sur.ReEnteredPassword)
                .NotEmpty()
                .WithMessage(IdentityErrorMessages.ReEnteredPasswordEmpty)
                .Must((request, reEnteredPassword) => reEnteredPassword == request.Password)
                .WithMessage(IdentityErrorMessages.ReEnteredPasswordNotMatching);
        }
    }
}
