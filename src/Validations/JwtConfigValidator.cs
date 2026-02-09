using System.Text;
using elastic_app_v3.Config;
using elastic_app_v3.Errors;
using FluentValidation;

namespace elastic_app_v3.Validations
{
    public sealed class JwtConfigValidator : AbstractValidator<JwtConfig>
    {
        public JwtConfigValidator()
        {
            RuleForPrivateKey();
            RuleForExpirationInMinutes();
            RuleForIssuer();
            RuleForAudience();
        }
        private void RuleForPrivateKey()
        {
            RuleFor(x => x.PrivateKey)
                .NotEmpty()
                .WithMessage(ErrorMessages.PrivateKeyEmpty);
        }
        private void RuleForExpirationInMinutes()
        {
            RuleFor(x => x.ExpirationInMinutes)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.ExpirationZeroOrNegative)
                .LessThanOrEqualTo(1440)
                .WithMessage(ErrorMessages.ExpirationTooLong);
        }
        private void RuleForIssuer()
        {
            RuleFor(x => x.Issuer)
                .NotEmpty()
                .WithMessage(ErrorMessages.IssuerEmpty);
        }
        private void RuleForAudience()
        {
            RuleFor(x => x.Audience)
                .NotEmpty()
                .WithMessage(ErrorMessages.AudienceEmpty);
        }
    }
}
