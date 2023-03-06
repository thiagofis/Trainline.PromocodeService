using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Threading.Tasks;
using Trainline.PromocodeService.Host.Configuration;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class CircuitBreakers
    {
        public const string Standard = "StandardConfiguration";

        public static IServiceCollection RegisterCircuitBreakerPolicies(this IServiceCollection services)
        {
            services.AddSingleton<IPolicyRegistry<string>>(services =>
            {
                var circuitBreakerSettings = services.GetRequiredService<IOptions<CircuitBreakerSettings>>().Value;

                return new PolicyRegistry
                {
                    [Standard] = HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TaskCanceledException>()
                        .Or<TimeoutException>()
                        .AdvancedCircuitBreakerAsync(circuitBreakerSettings[Standard])

                };
            });
            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(services => services.GetRequiredService<IPolicyRegistry<string>>());

            return services;
        }

        private static AsyncCircuitBreakerPolicy<TResult> AdvancedCircuitBreakerAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, CircuitBreakerSettings.CircuitBreakerPolicySettings circuitBreakerPolicySettings)
        {
            return policyBuilder.AdvancedCircuitBreakerAsync(
                circuitBreakerPolicySettings.FailureThreshold,
                circuitBreakerPolicySettings.SamplingDuration,
                circuitBreakerPolicySettings.MinimumThroughput,
                circuitBreakerPolicySettings.DurationOfBreak);
        }
    }
}
