using elastic_app_v3.Constants;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using FluentValidation;

namespace elastic_app_v3.Validations
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
                .WithMessage(ErrorMessages.FirstNameEmpty)
                .Matches("^[a-zA-z]+$")
                .WithMessage(ErrorMessages.FirstNameNonAlphabetical);
        }
        public void RuleForLastName()
        {
            RuleFor(sur => sur.LastName)
                .NotEmpty()
                .WithMessage(ErrorMessages.LastNameEmpty)
                .Matches("^[a-zA-z]+$")
                .WithMessage(ErrorMessages.LastNameNonAlphabetical);
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
                .WithMessage(ErrorMessages.PasswordTooLongMessage()); // to do: think of validations for password
        }
        public void RuleForReEnteredPassword()
        {
            RuleFor(sur => sur.ReEnteredPassword)
                .NotEmpty()
                .WithMessage(ErrorMessages.ReEnteredPasswordEmpty)
                .Must((request, reEnteredPassword) => reEnteredPassword == request.Password)
                .WithMessage(ErrorMessages.ReEnteredPasswordNotMatching);
        }
    }
}
