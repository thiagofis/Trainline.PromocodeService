using System;
using Trainline.Acceptance;

namespace Trainline.PromocodeService.Acceptance.Helpers
{
    public class HostTestContext
    {
        public ApiClient ApiClient { get; set; }
        public Uri ContextUri { get; set; }
        public string ConversationId { get; set; }
        public Uri BaseAddress { get; set; }

        public HostTestContext()
        {
            ContextUri = new Uri("http://contexturi.com");
            ConversationId = Guid.NewGuid().ToString();
        }
    }
}
