using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Steps;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public class InMemoryApplicationFactory : WebApplicationFactory<Startup.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    services.RegisterSteps();
                    services.RegisterStubs();
                })
                .UseEnvironment("InMemoryTests");
        }
    }
}
