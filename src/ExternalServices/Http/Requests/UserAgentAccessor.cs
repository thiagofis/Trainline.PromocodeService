using System.Reflection;

namespace Trainline.PromocodeService.ExternalServices.Http.Requests
{

    public static class UserAgentAccessor
    {
        public static string GetUserAgent() => $"PromocodeService/{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
    }
}
