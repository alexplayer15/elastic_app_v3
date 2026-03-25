using FluentValidation;
using elastic_app_v3.application.Validations;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.SingUp;
using elastic_app_v3.application.Services.Payments;
using elastic_app_v3.application.Services.Identity;

namespace elastic_app_v3.application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<SignUpRequest>(ServiceLifetime.Scoped);
            services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
