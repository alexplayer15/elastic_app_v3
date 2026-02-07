using elastic_app_v3.DTOs;
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
                .WithMessage("FirstName cannot be empty")
                .Matches("^[a-zA-z]+$")
                .WithMessage("You can only include alphabetical characters in your FirstName");
        }
        public void RuleForLastName()
        {
            RuleFor(sur => sur.LastName)
                .NotEmpty()
                .WithMessage("LastName cannot be empty")
                .Matches("^[a-zA-z]+$")
                .WithMessage("You can only include alphabetical characters in your LastName");
        }

        public void RuleForUserName()
        {
            RuleFor(sur => sur.UserName)
                .NotEmpty()
                .WithMessage("UserName cannot be empty")
                .MinimumLength(6)
                .MaximumLength(15)
                .WithMessage("UserName must be between 6 and 15 characters long");
        }

        public void RuleForPassword()
        {
            RuleFor(sur => sur.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty")
                .MinimumLength(8)
                .MaximumLength(22)
                .WithMessage("Password must be between 8 and 22 characters long"); // to do: think of validations for password
        }

        public void RuleForReEnteredPassword()
        {
            RuleFor(sur => sur)
                .Must(sur => sur.Password == sur.ReEnteredPassword)
                .WithMessage("Password must match ReEnteredPassword");
        }
    }
}
