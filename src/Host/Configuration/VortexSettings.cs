using System;

namespace Trainline.PromocodeService.Host.Configuration
{
    public class VortexSettings
    {
        public Uri PublishUri { get; set; }
        public string Stream { get; set; }
        public bool DiagnosticsEnabled { get; set; }
    }
}