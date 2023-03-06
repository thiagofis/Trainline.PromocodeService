using Trainline.PromocodeService.Acceptance.Helpers.TestContracts;

namespace Trainline.PromocodeService.Acceptance.Helpers.Steps
{
    public class RequestOptions
    {
        public string ContentType { get; set; } = "application/json";
        public bool IncludeConversationIdHeader { get; set; } = true;
        public bool IncludeContextUriHeader { get; set; } = true;
        public bool IncludeUserAgentHeader { get; set; } = true;
        public string Accept { get; set; } = MediaTypes.PromocodeV1;
    }
}
