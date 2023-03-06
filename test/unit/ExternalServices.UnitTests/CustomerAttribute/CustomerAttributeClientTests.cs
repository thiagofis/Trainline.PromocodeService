using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace ExternalServices.UnitTests.CustomerAttribute
{
    [TestFixture]
    public class CustomerAttributeClientTests
    {
        private CustomerAttributeClient _sut;
        private Mock<IHttpRequestClient> _httpRequestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _httpRequestClient = new Mock<IHttpRequestClient>();
            var option = Options.Create(new CustomerAttributeSettings {BaseUri = "http://test"});
            var logger = new Mock<ILogger<CustomerAttributeClient>>();

            _sut = new CustomerAttributeClient(_httpRequestClient.Object, option, logger.Object);
        }

        [Test]
        public async Task GivenACustomerIdDoesNotExist_WhenGetCustomerAttributes_ThenReturnsTheDefaultValues()
        {
            var customerId = "123456";
            _httpRequestClient.Setup(x => x.GetAsync<CustomerAttributes>(It.IsAny<Uri>()))
                .Throws(new HttpResponseException(new Uri("http://t"), HttpStatusCode.NotFound, string.Empty));

            var result = await _sut.GetCustomerAttributes(customerId);

            Assert.AreEqual(customerId, result.CustomerId);
            Assert.True(result.GetValue<bool?>( CustomerAttributeNames.IsNewCustomer));
            Assert.False(result.GetValue<bool?>( CustomerAttributeNames.BoughtRailcard));
            Assert.False(result.GetValue<bool?>( CustomerAttributeNames.IsActive));
        }
    }
}
