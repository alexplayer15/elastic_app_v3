using Microsoft.AspNetCore.Mvc.Testing;

namespace elastic_app_v3.integration.tests.SetUp
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
    }
}
