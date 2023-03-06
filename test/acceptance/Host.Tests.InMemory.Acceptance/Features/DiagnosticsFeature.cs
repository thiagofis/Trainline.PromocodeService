using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers.TestContracts.Diagnostics;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{
    [TestFixture]
    public class DiagnosticsFeature : AcceptanceTestBase
    {
        [Test]
        public async Task GetDiagnostics()
        {
            var response = await When.IGet<ServiceInformationContract>("/diagnostics/healthcheck");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.OK);

            Assert.IsNull(response.Content.Errors);
            Assert.AreEqual(AppSettings.ApplicationName, response.Content.ServiceName);
            Assert.AreEqual(AppSettings.ApplicationId, response.Content.ApplicationId);
            Assert.IsTrue(response.Content.IsHealthy);
        }
    }
}
