using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Controllers;
using Trainline.PromocodeService.Host.Vortex;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.UnitTests.Controllers
{
    [TestFixture]
    public class CampaignControllerTests
    {
        private CampaignController _sut;
        private Mock<ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData>> _campaignValidator;
        private Mock<IVortexEventPublisher> _publisher;
        private Mock<IHeaderService> _headerService;

        [SetUp]
        public void Setup()
        {
            _campaignValidator = new Mock<ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData>>();
            _publisher = new Mock<IVortexEventPublisher>();
            _headerService = new Mock<IHeaderService>();

            _campaignValidator.Setup(x => x.ValidateEligibility(It.IsAny<NewCustomerCampaignApplication>()))
                .ReturnsAsync(new NewCustomerCampaignEligibilityData("64", "delpiero@juve.it", "Alex", "Del Piero", "VoucherifyCampaignId", "en-gb"));

            _sut = new CampaignController(_campaignValidator.Object, _publisher.Object, _headerService.Object);
        }

        [Test]
        public async Task GivenAnApplicationOfTheNewCustomerCampaign_WhenItIsRequested_ThenTheApplicationOfTheCampaignIsPerformed()
        {
            var request = new ApplyNewCustomerCampaign
            {
                Email = "delpiero@juve.it",
                FirstName = "Alex",
                LastName = "Del Piero",
                Locale = "en-gb"
            };

            var response = await _sut.NewCustomer(request);

            _campaignValidator.Verify(x => x.ValidateEligibility(It.IsAny<NewCustomerCampaignApplication>()), Times.Once);
        }

        [Test]
        public async Task GivenTheCustomerIsEligibleForTheCustomerCampaign_WhenItIsRequested_ThenTheNewCustomerPromocodeVortexMessageIsRaised()
        {
            var request = new ApplyNewCustomerCampaign
            {
                Email = "delpiero@juve.it",
                FirstName = "Alex",
                LastName = "Del Piero",
                Locale = "en-gb"
            };

            var response = await _sut.NewCustomer(request);

            _publisher.Verify(x => x.Publish(It.IsAny<NewCustomerPromocode>()), Times.Once);
        }

        [Test]
        public void GivenTheCustomerIsNotEligibleForTheCustomerCampaign_WhenItIsRequested_ThenTheNewCustomerPromocodeVortexMessageIsNeverRaised()
        {
            var request = new ApplyNewCustomerCampaign
            {
                Email = "delpiero@juve.it",
                FirstName = "Alex",
                LastName = "Del Piero",
                Locale = "en-gb"
            };

            _campaignValidator.Setup(x => x.ValidateEligibility(It.IsAny<NewCustomerCampaignApplication>()))
                              .Throws(new CustomerIsNotEligibleForTheCampaignException("The customer is not eligible for the campaign."));

            AsyncTestDelegate action =  () => _sut.NewCustomer(request);

            Assert.ThrowsAsync<CustomerIsNotEligibleForTheCampaignException>(action);
            _publisher.Verify(x => x.Publish(It.IsAny<NewCustomerPromocode>()), Times.Never);
        }

        [Test]
        public async Task GivenAnApplicationOfTheNewCustomerCampaign_WhenItIsRequested_ThenTheAcceptedResponseIsReturned()
        {
            var request = new ApplyNewCustomerCampaign
            {
                Email = "delpiero@juve.it",
                FirstName = "Alex",
                LastName = "Del Piero",
                Locale = "en-gb"
            };

            var response = await _sut.NewCustomer(request);

            Assert.IsInstanceOf<AcceptedResult>(response);
        }
    }
}
