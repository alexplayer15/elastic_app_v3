using FluentValidation;
using elastic_app_v3.application.Services;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Validations;

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

            return services;
        }
    }
}
