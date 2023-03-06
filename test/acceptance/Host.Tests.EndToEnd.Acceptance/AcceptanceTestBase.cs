using System.Net.Http;
using AutoFixture;
using NUnit.Framework;
using Trainline.Acceptance;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance
{
    public abstract class AcceptanceTestBase
    {
        private ApiClient ApiClient { get; set; }
        private Fixture Fixture { get; set; }
        protected GivenSteps Given { get; private set; }
        protected ThenSteps Then { get; private set; }
        protected WhenSteps When { get; private set; }
        protected AppSettings AppSettings { get; private set; }
        protected HostTestContext Context { get; set; }


        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            AppSettings = ConfigurationService.Load();
            Context = new HostTestContext();
        }

        [SetUp]
        public void BaseSetup()
        {
            Fixture = new Fixture();

            var httpClient = new HttpClient
            {
                BaseAddress = AppSettings.ExternalAddress
            };

            ApiClient = ApiClientFactory.For(httpClient);

            Context.ApiClient = ApiClient;

            Given = new GivenSteps(Fixture);
            When = new WhenSteps(Context);
            Then = new ThenSteps();
        }

        [TearDown]
        public void Dispose()
        {
            ApiClient?.Dispose();
        }
    }
}
