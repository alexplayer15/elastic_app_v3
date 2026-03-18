using System.Text;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Repositories;
using elastic_app_v3.infrastructure.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace elastic_app_v3.infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            services.AddHealthChecks();
            services.ConfigureOptions(configuration);
            services.AddResilienceConfiguration(configuration);

            return services;
        }
        private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserSettings>(configuration.GetSection(UserSettings.UserAppSettingsName));
            services.Configure<PaymentSettings>(configuration.GetSection(PaymentSettings.PaymentAppSettingsName));

            services.AddOptions<JwtConfigOptions>()
                .Bind(configuration.GetSection(JwtConfigOptions.JwtConfig))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtConfigOptions>>((options, jwtConfigOptions) =>
                {
                    var jwtConfig = jwtConfigOptions.Value;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtConfig.PrivateKey)),
                    };
                });

            services.AddOptions<UserResiliencePolicy>(UserResiliencePolicy.UserResiliencePolicySettings)
                .Bind(configuration.GetSection(UserResiliencePolicy.UserResiliencePolicySettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
        private static IServiceCollection AddResilienceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddResiliencePipeline(UserResiliencePolicy.UserResiliencePolicyKey, (builder, context) =>
            {
                var options = context.GetOptions<UserResiliencePolicy>(UserResiliencePolicy.UserResiliencePolicySettings);

                builder
                    .AddTimeout(TimeSpan.FromSeconds(options.TimeoutInMilliseconds))
                    .AddRetry(new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder()
                            .Handle<SqlException>()
                            .Handle<TimeoutException>(),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        MaxRetryAttempts = options.MaxRetryAttempts,
                        Delay = TimeSpan.FromSeconds(options.Delay)
                        //add observability on retry attempts and delays between retries
                    })
                    .AddTimeout(TimeSpan.FromSeconds(options.InnerTimeoutInMilliseconds))
                    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                    {
                        StateProvider = new CircuitBreakerStateProvider(),
                        ShouldHandle = new PredicateBuilder()
                            .Handle<SqlException>()
                            .Handle<TimeoutException>(),
                        FailureRatio = options.FailureRatio,
                        SamplingDuration = TimeSpan.FromSeconds(options.SampleDuration),
                        MinimumThroughput = options.MinimumThroughput,
                        BreakDuration = TimeSpan.FromSeconds(options.BreakDuration),
                        //to do: add observability on state of circuit breaker
                    });
            });
        }
    }
}
