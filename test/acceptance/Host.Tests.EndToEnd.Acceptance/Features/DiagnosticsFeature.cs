using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers.TestContracts.Diagnostics;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Features
{

    [TestFixture]
    public class DiagnosticsFeature : AcceptanceTestBase
    {
         [Test]
        public async Task GetDiagnostics()
        {
            var response = await When.IGet<ServiceInformationContract>("/ping");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.OK);

            Assert.IsNull(response.Content.Errors);
            Assert.IsNotNull(response.Content.ServiceName);
            Assert.IsNotNull(response.Content.ApplicationId);
            Assert.IsTrue(response.Content.IsHealthy);
        }
    }
}
