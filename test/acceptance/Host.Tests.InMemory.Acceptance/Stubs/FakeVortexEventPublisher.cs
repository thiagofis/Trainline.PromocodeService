using System.Threading.Tasks;
using Trainline.VortexPublisher;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class FakeVortexEventPublisher : IVortexEventPublisher
    {
        public void Dispose() { }

        public Task<PublishResponse> Publish<T>(T evnt) where T : VortexEvent => Task.FromResult(new PublishResponse());


        public Task<PublishResponse> Publish<T>(T evnt, string streamName) where T : VortexEvent => Task.FromResult(new PublishResponse());
    }
}
