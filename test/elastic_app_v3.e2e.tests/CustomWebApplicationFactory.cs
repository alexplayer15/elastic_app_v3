using Microsoft.AspNetCore.Mvc.Testing;

namespace elastic_app_v3.e2e.tests;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
where TStartup : class
{
}
