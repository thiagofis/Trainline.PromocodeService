using System.Net.Http;

namespace Trainline.PromocodeService.Acceptance.Helpers.TestContracts
{
    public class Relation
    {
        public string Name { get; set; }
        public string ActionName { get; set; }
        public HttpMethod Method { get; set; }
    }
}
