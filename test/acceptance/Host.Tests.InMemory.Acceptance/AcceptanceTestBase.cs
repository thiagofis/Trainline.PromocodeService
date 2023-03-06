using System;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Trainline.Acceptance;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Steps;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public abstract class AcceptanceTestBase
    {
        private ApiClient ApiClient { get; set; }
        private Fixture Fixture { get; set; }
        protected InMemoryGivenSteps Given { get; private set; }
        protected ThenSteps Then { get; private set; }
        protected InMemoryVoucherifyClient Voucherify { get; private set; }
        protected WhenSteps When { get; private set; }
        protected ServiceSettings AppSettings { get; set; }
        protected HostTestContext Context { get; private set; }

        private InMemoryApplicationFactory _factory;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            _factory = new InMemoryApplicationFactory();
            ApiClient = ApiClientFactory.For(_factory.CreateClient());

            var scope = _factory.Services.CreateScope();

            AppSettings = scope.ServiceProvider.GetService<IOptions<ServiceSettings>>().Value;

            Given = scope.ServiceProvider.GetService<InMemoryGivenSteps>();
            When = scope.ServiceProvider.GetService<WhenSteps>();
            Then = scope.ServiceProvider.GetService<ThenSteps>();

            Voucherify = scope.ServiceProvider.GetService<InMemoryVoucherifyClient>();

            Context = scope.ServiceProvider.GetService<HostTestContext>();
            Context.ApiClient = ApiClient;
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        protected void Dispose()
        {
            ApiClient?.Dispose();
            _factory?.Dispose();
        }
    }
}
