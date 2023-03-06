using System;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance
{
    public class AppSettings
    {
        public Uri ExternalAddress { get; set; }
        public Uri ContextUri { get; set; }
        public Uri DummyInventoryUri { get; set; }
        public string TestPromocode { get; set; }
    }
}
