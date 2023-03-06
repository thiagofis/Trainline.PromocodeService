using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Trainline.PromocodeService.Host.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsInMemoryTests(this IWebHostEnvironment env)
        {
            return env.IsEnvironment("InMemoryTests");
        }
    }
}
