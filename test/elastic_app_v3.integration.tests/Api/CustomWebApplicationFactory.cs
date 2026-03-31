using Microsoft.AspNetCore.Mvc.Testing;

namespace elastic_app_v3.integration.tests.Api;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
where TStartup : class
{

}
