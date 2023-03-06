using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Trainline.PromocodeService.ExternalServices.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent httpContent, JsonSerializerSettings serializerSettings)
        {
            using var stream = await httpContent.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var reader = new JsonTextReader(streamReader);

            var serializer = JsonSerializer.Create(serializerSettings);
            return serializer.Deserialize<T>(reader);
        }
    }
}
