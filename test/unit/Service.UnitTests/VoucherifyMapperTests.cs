using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Mappers;

namespace Service.UnitTests
{
    [TestFixture]
    public class VoucherifyMapperTests
    {
        private InvoiceInfo[] _invoices;
        private CustomerVoucherifyMapper _customerVoucherifyMapper;
        private VoucherifyMapper _mapper;
        private Mock<ICustomerServiceClient> _customerServiceClientMock;
        private Mock<ICustomerAttributeClient> _customerAttributeClientMock;

        [SetUp]
        public void Setup()
        {
            _invoices = new InvoiceInfo[]
            {
                new InvoiceInfo
                {
                    Id = "Invoice",
                    CurrencyCode = "GBP",
                    ProductItems = new List<ProductItem>
                    {
                        new ProductItem
                        {
                            ProductId = "one",
                            ProductUri = new Uri("http://www.products.com/one"),
                            Amount = 50m
                        }
                    }
                }
            };
           _customerServiceClientMock = new Mock<ICustomerServiceClient>();
           _customerAttributeClientMock = new Mock<ICustomerAttributeClient>();
           _customerVoucherifyMapper = new CustomerVoucherifyMapper(_customerServiceClientMock.Object, _customerAttributeClientMock.Object);
           _mapper = new VoucherifyMapper(_customerVoucherifyMapper);
        }

        [Test]
        public async Task Map_CustomerUriSuppliedAndCustomerIsNewCustomer_CustomerIsNewStatusIsNewCustomer()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributesDetails = new CustomerAttributeDetails
            {
                Name = "isNewCustomer",
                DataType = "boolean",
                Value = true,
                LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                ProvenanceSource = "c99.kronos.private.Smtm",
                ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });


            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId))
                .ReturnsAsync(new Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract.CustomerAttributes { CustomerId = customerId, Attributes = new CustomerAttributeDetails[] { customerAttributesDetails } });


            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("newCustomer", mapped.Customer.metadata.isnew_status);
        }

        [Test]
        public async Task Map_CustomerUriSuppliedAndCustomerIsRepeatCustomer_CustomerIsNewStatusIsRepeatCustomer()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";

            var customerAttributesDetails = new CustomerAttributeDetails
            {
                Name = "isNewCustomer",
                DataType = "boolean",
                Value = false,
                LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                ProvenanceSource = "c99.kronos.private.Smtm",
                ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });

            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId))
                .ReturnsAsync(new Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract.CustomerAttributes { CustomerId = customerId, Attributes = new CustomerAttributeDetails[] { customerAttributesDetails } } );

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("repeatCustomer", mapped.Customer.metadata.isnew_status);
        }

        [Test]
        public async Task Map_CustomerUriNotSupplied_CustomerIsNewStatusIsNotIdentified()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customerId,
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
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });
            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId)).ReturnsAsync(customerAttributes);

            var mapped = await _mapper.Map(_invoices, null);

            Assert.AreEqual("customerNotIdentified", mapped.Customer.metadata.isnew_status);
        }

        [Test]
        public async Task Map_CustomerUriSuppliedAndCustomerIsFirstTimeRailCardBuy_CustomerRailcardStatusIsFirsTime()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customerId,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "boughtRailcard",
                        DataType = "boolean",
                        Value = false,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });
            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId)).ReturnsAsync(customerAttributes);

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("firstTime", mapped.Customer.metadata.railcard_status);
        }

        [Test]
        public async Task Map_CustomerUriSuppliedAndCustomerHasBoughtRailcard_CustomerRailcardStatusIsHasBoughtRailcard()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customerId,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "boughtRailcard",
                        DataType = "boolean",
                        Value = true,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });
            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId)).ReturnsAsync(customerAttributes);

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("hasBoughtRailcard", mapped.Customer.metadata.railcard_status);
        }

        [Test]
        public async Task Map_isNewCustomerNotAvailable_CustomerIsNewStatus()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });

            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId))
                .ReturnsAsync(new Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract.CustomerAttributes { CustomerId = customerId, Attributes = new CustomerAttributeDetails[0] });

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("newCustomer", mapped.Customer.metadata.isnew_status);
        }

        [Test]
        public async Task Map_CustomerIsNotActive_CustomerActivityStatusIsDisabled()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customerId,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "isActive",
                        DataType = "boolean",
                        Value = false,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });
            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId)).ReturnsAsync(customerAttributes);

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("disabled", mapped.Customer.metadata.activity_status);
        }

        [Test]
        public async Task Map_CustomerIsctive_CustomerActivityStatusIsActive()
        {
            var customerUri = new Uri("http://customeruri");

            var customerId = "1";
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customerId,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        Name = "isActive",
                        DataType = "boolean",
                        Value = true,
                        LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                        ProvenanceSource = "c99.kronos.private.Smtm",
                        ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                    }
                }
            };

            _customerServiceClientMock.Setup(c => c.GetCustomer(customerUri)).ReturnsAsync(new Trainline.PromocodeService.ExternalServices.Customer.Contract.Customer { Id = customerId });
            _customerAttributeClientMock.Setup(c => c.GetCustomerAttributes(customerId)).ReturnsAsync(customerAttributes);

            var mapped = await _mapper.Map(_invoices, customerUri);

            Assert.AreEqual("active", mapped.Customer.metadata.activity_status);
        }
    }
}
