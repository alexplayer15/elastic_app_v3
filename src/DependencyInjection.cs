using elastic_app_v3.Repositories;
using elastic_app_v3.Services;
using elastic_app_v3.Config;
using FluentValidation;
using elastic_app_v3.DTOs;
using elastic_app_v3.Validations;
using elastic_app_v3.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.Domain;

namespace elastic_app_v3
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<SignUpRequest>(ServiceLifetime.Scoped);
            services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<IUserRepository, UserRepository>();

            return services;
        }
        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserSettings>(configuration.GetSection("UserSettings"));

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

            return services;
        }
        public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            //.AddApiExplorer(options =>
            //{
            //    options.GroupNameFormat = "'v'V";
            //    options.SubstituteApiVersionInUrl = true;
            //});

            return services;
        }
    }
}
