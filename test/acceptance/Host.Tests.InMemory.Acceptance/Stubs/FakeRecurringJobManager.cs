using Hangfire;
using Hangfire.Common;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class FakeRecurringJobManager : IRecurringJobManager
    {
        public void AddOrUpdate(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options)
        {
        }

        public void Trigger(string recurringJobId)
        {
        }

        public void RemoveIfExists(string recurringJobId)
        {
        }
    }
}
