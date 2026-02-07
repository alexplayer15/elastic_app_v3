using elastic_app_v3.DTOs;
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
            RuleFor(lr => lr.UserName)
                .NotEmpty()
                .WithMessage("UserName cannot be empty")
                .MinimumLength(6)
                .MaximumLength(15)
                .WithMessage("UserName must be between 6 and 15 characters long");
        }
        public void RuleForPassword()
        {
            RuleFor(lr => lr.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty")
                .MinimumLength(8)
                .MaximumLength(22)
                .WithMessage("Password must be between 8 and 22 characters long"); // to do: think of validations for password
        }
    }
}
