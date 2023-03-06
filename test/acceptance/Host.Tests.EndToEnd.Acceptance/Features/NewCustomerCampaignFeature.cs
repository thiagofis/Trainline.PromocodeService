using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Acceptance.Helpers.TestContracts;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Features
{
    [Category("IgnoreOnC22")]
    public class NewCustomerCampaignFeature : AcceptanceTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Context.ContextUri = ContextUriCreator.Create(AppSettings.ContextUri);
        }

        [Test]
        public async Task GiveAnUnregisteredCustomer_WhenApplyToNewCustomerCampaign_ThenTheAcceptedHttpCodeIsReturned()
        {
            var unregisteredEmail = $"{Guid.NewGuid()}@acceptance.testing.com";
            var request = new ApplyNewCustomerCampaign
            {
                Email = unregisteredEmail,
                FirstName = $"FirstName-{Guid.NewGuid()}",
                LastName = $"LastName-{Guid.NewGuid()}",
                ExternalCampaignId = "Acceptance_Testing_Id",
                Locale = "en-gb"
            };

            var response = await When.IPost<ApplyNewCustomerCampaign, AcceptedResult>(request, "/campaign/newCustomer");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.Accepted);
        }

        [Test]
        public async Task GivenAnInvalidCustomerEmail_WhenApplyToNewCustomerCampaign_ThenTheCustomerImplicitRegistrationFailedErrorIsReturned()
        {
            var invalidEmail = "invalid-email";
            var request = new ApplyNewCustomerCampaign
            {
                Email = invalidEmail,
                FirstName = $"FirstName-{Guid.NewGuid()}",
                LastName = $"LastName-{Guid.NewGuid()}",
                ExternalCampaignId = "Acceptance_Testing_Id",
                Locale = "en-gb"
            };

            var response = await When.IPost<ApplyNewCustomerCampaign, ResponseBase>(request, "/campaign/newCustomer");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);
            Then.TheResponseShouldHaveAnError(response, ErrorCodes.CustomerImplicitRegistrationFailed);
        }
    }
}
