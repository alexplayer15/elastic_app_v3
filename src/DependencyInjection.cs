using elastic_app_v3.Repositories;
using elastic_app_v3.Services;
using elastic_app_v3.Config;
using FluentValidation;
using elastic_app_v3.DTOs;
using elastic_app_v3.Validations;
using Amazon.SecretsManager;
using Microsoft.Extensions.Options;
using Amazon.Runtime;
using elastic_app_v3.Tokens;
using elastic_app_v3.SecretsManager;

namespace elastic_app_v3
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserSettings>(configuration.GetSection("UserSettings"));
            services.Configure<SecretsManagerSettings>(configuration.GetSection("SecretsManagerSettings"));
            services.AddValidatorsFromAssemblyContaining<SignUpRequest>(ServiceLifetime.Scoped);
            services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<ISecretsManagerClient, SecretsManagerClient>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.ConfigureSecretsManager();

            return services;
        }
        public static IServiceCollection ConfigureSecretsManager(this IServiceCollection services)
        {
            return services.AddSingleton<IAmazonSecretsManager>(sp =>
            {
                var secretsManagerSettings = sp
                    .GetRequiredService<IOptions<SecretsManagerSettings>>()
                    .Value;

                var secretsManagerConfiguration = new AmazonSecretsManagerConfig
                {
                    ServiceURL = secretsManagerSettings.ServiceUrl,
                    AuthenticationRegion = secretsManagerSettings.AuthenticationRegion,
                };

                return new AmazonSecretsManagerClient(
                    new BasicAWSCredentials("DUMMYACCESSKEY", "DUMMYSECRETKEY"),
                    secretsManagerConfiguration);
            });
        }
    }
}
