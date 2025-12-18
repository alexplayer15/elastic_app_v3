using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using elastic_app_v3.Repositories;

namespace elastic_app_v3.integration.tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
        public IUserRepository PaymentsRepository { get; private set; } = new UserRepository();
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSolutionRelativeContentRoot(AppContext.BaseDirectory);
            builder.UseEnvironment("Development");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IUserRepository>();
                services.AddSingleton<IUserRepository>(PaymentsRepository);
            });

            base.ConfigureWebHost(builder);
        }

        [CollectionDefinition("Integration Tests")]
        public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
        {
        }
    }
}
