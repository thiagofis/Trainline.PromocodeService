using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices;
using Trainline.PromocodeService.ExternalServices.Context;
using Trainline.PromocodeService.ExternalServices.Context.Contract;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Exceptions;

namespace Service.UnitTests
{
    public class NewCustomerCampaignValidatorTests
    {
        private Mock<IContextClient> _contextClient;
        private Mock<ICustomerServiceClient> _customerClient;
        private Mock<ICustomerAttributeClient> _customerAttributeClient;
        private Mock<ILogger<NewCustomerCampaignValidator>> _logger;


        private NewCustomerCampaignValidator _sut;

        [SetUp]
        public void Setup()
        {
            _contextClient = new Mock<IContextClient>();
            _customerClient = new Mock<ICustomerServiceClient>();
            _customerAttributeClient = new Mock<ICustomerAttributeClient>();
            _logger = new Mock<ILogger<NewCustomerCampaignValidator>>();

            _contextClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ContextParameters {Values =  new ContextValues {ApplicationManagedGroupId = new []{"64"}, ApplicationEntryPointId = new []{"20"}}});

            _customerClient.Setup(x =>
                    x.RegisterCustomerImplicitly(It.IsAny<ImplicitRegistration>()))
                .ReturnsAsync(new CustomerRegistered {CustomerId = "101"});


            _sut = new NewCustomerCampaignValidator(
                _contextClient.Object,
                _customerClient.Object,
                _customerAttributeClient.Object,
                _logger.Object);
        }

        [Test]
        public async Task
            GivenTheCustomerIsEligibleForTheCampaign_WhenTheApplicationIsValidated_ThenTheCampaignEligibilityDataIsReturned()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenOneCustomerIsFound();
            GivenCustomerIsEligibleForTheCampaign();

            var result = await _sut.ValidateEligibility(application);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewCustomerCampaignEligibilityData>(result);
        }

        [Test]
        public async Task
            GivenCustomerIsHasNoInformationAboutIsNewCustomer_WhenTheApplicationIsValidated_ThenTheCampaignEligibilityDataIsReturned()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenOneCustomerIsFound();
            GivenCustomerIsHasNoInformationAboutIsNewCustomer();

            var result = await _sut.ValidateEligibility(application);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewCustomerCampaignEligibilityData>(result);
        }


        [Test]
        public void
            GivenTheCustomerIsNotEligibleForTheCampaign_WhenTheApplicationIsValidated_ThenTheCustomerIsNotEligibleForTheCampaignExceptionIsRaised()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenOneCustomerIsFound();
            GivenCustomerIsNotEligibleForTheCampaign();

            AsyncTestDelegate action = () => _sut.ValidateEligibility(application);

            Assert.ThrowsAsync<CustomerIsNotEligibleForTheCampaignException>(action);
        }


        [Test]
        public async Task
            GivenTheCustomerDoesNotExist_WhenTheApplicationIsValidated_ThenTheCustomerIsImplicitlyRegistered()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenNoCustomersAreFound();

            var result = await _sut.ValidateEligibility(application);

            _customerClient.Verify(
                x => x.RegisterCustomerImplicitly(
                    It.Is<ImplicitRegistration>(it => it.EmailAddress == application.Email)), Times.Once);
        }

        [Test]
        public async Task
            GivenTheCustomerDoesNotExist_WhenTheApplicationIsValidated_ThenTheValidateCustomerEligibleForTheCampaignIsNotPerformed()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenNoCustomersAreFound();

            var result = await _sut.ValidateEligibility(application);

            _customerAttributeClient.Verify(
                x => x.GetCustomerAttributes(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task
            GivenTheCustomerDoesNotExist_WhenTheApplicationIsValidated_ThenTheCampaignEligibilityDataIsReturnedd()
        {
            var application = GrivenANewCustomerCampaignApplication();
            GivenNoCustomersAreFound();

            var result = await _sut.ValidateEligibility(application);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewCustomerCampaignEligibilityData>(result);
        }

        private static NewCustomerCampaignApplication GrivenANewCustomerCampaignApplication()
        {
            return new NewCustomerCampaignApplication(
                "alex@delpiero.it",
                "Alex",
                "Del Piero",
                "VoucherifyCampaignId",
                "en-gb",
                "http://context-uri.test",
                "conversation-id"
            );
        }

        private void GivenOneCustomerIsFound()
        {
            var customersFound = new CustomersFound
            {
                Customers = new Customer[]
                {
                    new Customer()
                    {
                        Id = "99"
                    }
                }
            };

            CreateGetCustomerByEmailStub(customersFound);
        }

        private void GivenNoCustomersAreFound() => CreateGetCustomerByEmailStub(new CustomersFound());

        private void CreateGetCustomerByEmailStub(CustomersFound customersFound)
        {
            _customerClient.Setup(x => x.GetCustomerByEmail(It.IsAny<SearchCriteria>()))
                .ReturnsAsync(customersFound);
        }

        private void GivenCustomerIsEligibleForTheCampaign()
        {
            CreateGetIsNewCustomerAttributeStub(new CustomerAttributes
            {
                CustomerId = Guid.NewGuid().ToString(),
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "isNewCustomer",
                        DataType = "boolean",
                        Value = true,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            });
        }

        private void GivenCustomerIsHasNoInformationAboutIsNewCustomer()
        {
            CreateGetIsNewCustomerAttributeStub(new CustomerAttributes
            {
                CustomerId = Guid.NewGuid().ToString(),
                Attributes = new CustomerAttributeDetails[] { }
            });
        }

        private void GivenCustomerIsNotEligibleForTheCampaign()
        {
            CreateGetIsNewCustomerAttributeStub(new CustomerAttributes
            {
                CustomerId = Guid.NewGuid().ToString(),
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "isNewCustomer",
                        DataType = "boolean",
                        Value = false,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            });
        }

        private void CreateGetIsNewCustomerAttributeStub(CustomerAttributes customerAttributes)
        {
            _customerAttributeClient
                .Setup(x => x.GetCustomerAttributes(It.IsAny<string>()))
                .ReturnsAsync(customerAttributes);
        }
    }
}
