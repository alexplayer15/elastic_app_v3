using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace elastic_app_v3.integration.tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSolutionRelativeContentRoot(AppContext.BaseDirectory);
            builder.UseEnvironment("Development");

            base.ConfigureWebHost(builder);
        }

        [CollectionDefinition("Integration Tests")]
        public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
        {
        }
    }
}
