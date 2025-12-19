using elastic_app_v3.Repositories;
using elastic_app_v3.Services;
using System.Data;
using Microsoft.Data.SqlClient;
using elastic_app_v3.Config;

namespace elastic_app_v3
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserSettings>(configuration.GetSection("UserSettings"));
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();

            return services;
        }
    }
}
