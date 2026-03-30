using FluentValidation;
using elastic_app_v3.application.Validations;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.Services.Payments;
using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Services.Profiles;

namespace elastic_app_v3.application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<SignUpRequest>(ServiceLifetime.Scoped);
            services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IProfileService, ProfileService>();

            return services;
        }
    }
}
