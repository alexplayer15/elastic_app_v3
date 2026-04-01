using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.application.Services.Payments;
using elastic_app_v3.application.Services.Profiles;
using FluentValidation;

namespace elastic_app_v3.application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IProfileService, ProfileService>();

        return services;
    }
}
