using System;
using System.Collections.Generic;
using static Trainline.PromocodeService.Host.Configuration.CircuitBreakerSettings;

namespace Trainline.PromocodeService.Host.Configuration
{
    public class CircuitBreakerSettings : Dictionary<string, CircuitBreakerPolicySettings>
    {
        public class CircuitBreakerPolicySettings
        {
            public double FailureThreshold { get; set; }
            public int MinimumThroughput { get; set; }
            public TimeSpan SamplingDuration { get; set; }
            public TimeSpan DurationOfBreak { get; set; }
        }
    }
}
