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
            services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>(); //singleton correct here?
            services.AddHealthChecks();
            services.ConfigureOptions(configuration);
            services.AddResilienceConfiguration(configuration);

            return services;
        }
        private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ElasticDatabaseSettings>(configuration.GetSection(ElasticDatabaseSettings.UserAppSettingsName));

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

            services.AddOptions<ResiliencePolicy>(ResiliencePolicy.UserResiliencePolicySettings)
                .Bind(configuration.GetSection(ResiliencePolicy.UserResiliencePolicySettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
        private static IServiceCollection AddResilienceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddResiliencePipeline(ResiliencePolicy.UserResiliencePolicyKey, (builder, context) =>
            {
                var options = context.GetOptions<ResiliencePolicy>(ResiliencePolicy.UserResiliencePolicySettings);

                var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("ResiliencePolicyLogger");

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
                        Delay = TimeSpan.FromSeconds(options.Delay),
                        OnRetry = retryArguments =>
                        {
                            logger.LogWarning(
                                "Retrying due to {ExceptionType}: {Message}. Attempt {RetryAttempt} of {MaxRetryAttempts}.",
                                retryArguments.Outcome.Exception?.GetType().Name,
                                retryArguments.Outcome.Exception?.Message,
                                retryArguments.AttemptNumber + 1,
                                options.MaxRetryAttempts);

                            return ValueTask.CompletedTask;
                        },
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
                        OnOpened = circuitBreakerArguments =>
                        {
                            logger.LogError(
                                circuitBreakerArguments.Outcome.Exception,
                                "Circuit breaker OPENED for {BreakDuration}",
                                circuitBreakerArguments.BreakDuration);

                            return ValueTask.CompletedTask;
                        },

                        OnClosed = args =>
                        {
                            logger.LogInformation("Circuit breaker CLOSED (normal operation resumed)");
                            return ValueTask.CompletedTask;
                        },

                        OnHalfOpened = args =>
                        {
                            logger.LogWarning("Circuit breaker HALF-OPEN (testing recovery)");
                            return ValueTask.CompletedTask;
                        }
                    });
            });
        }
    }
}
