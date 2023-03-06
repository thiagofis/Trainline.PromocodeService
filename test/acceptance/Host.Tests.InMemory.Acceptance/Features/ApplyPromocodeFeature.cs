using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{
    [TestFixture]
    public class ApplyPromocodeFeature : AcceptanceTestBase
    {
        private string Code = "testCode123";
        private string PromocodeId = "PromocodeId123";

        [Test]
        public async Task Apply()
        {
            var productId = "Product_123";
            var productUri = new Uri($"https://product.com/{productId}");
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };

            Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(productUri);

            var response = await When.IPost<Invoice[], Contract.DiscountItem[]>(invoices, $"/promocodes/{PromocodeId}/apply");

            var discountItem = response.Content.Single();
            Assert.AreEqual(productId, discountItem.ProductId);
            Assert.AreEqual("invoice_123", discountItem.InvoiceId);
            Assert.AreEqual("ATOC", discountItem.Vendor);
            Assert.AreEqual(-10, discountItem.Amount);
        }

        [Test]
        public async Task Apply_MultipleProducts()
        {
            var product1Id = "Product_123";
            var product1Uri = new Uri($"https://product.com/{product1Id}");
            var product2Id = "Product_456";
            var product2Uri = new Uri($"https://product.com/{product2Id}");
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = product1Id,
                            ProductUri = product1Uri,
                            Vendor = "ATOC"
                        },
                        new Item
                        {
                            Amount = 100,
                            ProductId = product2Id,
                            ProductUri = product2Uri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };

            Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(product1Uri);
            Given.ATravelProtocolProduct(product2Uri);

            var response = await When.IPost<Invoice[], Contract.DiscountItem[]>(invoices, $"/promocodes/{PromocodeId}/apply");

            var discountItem = response.Content.Single(x => x.ProductId == product1Id);
            Assert.AreEqual("invoice_123", discountItem.InvoiceId);
            Assert.AreEqual("ATOC", discountItem.Vendor);
            Assert.AreEqual(-5, discountItem.Amount);

            discountItem = response.Content.Single(x => x.ProductId == product2Id);
            Assert.AreEqual("invoice_123", discountItem.InvoiceId);
            Assert.AreEqual("ATOC", discountItem.Vendor);
            Assert.AreEqual(-5, discountItem.Amount);
        }

        [Test]
        public async Task Apply_MultipleProductsAndTiredVoucher()
        {
            var product1Id = "Product_123";
            var product1Uri = new Uri($"https://product.com/{product1Id}");
            var product2Id = "Product_456";
            var product2Uri = new Uri($"https://product.com/{product2Id}");
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = product1Id,
                            ProductUri = product1Uri,
                            Vendor = "ATOC"
                        },
                        new Item
                        {
                            Amount = 100,
                            ProductId = product2Id,
                            ProductUri = product2Uri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };

            Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10, 200, 30);
            Given.ATravelProtocolProduct(product1Uri);
            Given.ATravelProtocolProduct(product2Uri);

            var response = await When.IPost<Invoice[], Contract.DiscountItem[]>(invoices, $"/promocodes/{PromocodeId}/apply");

            var discountItem = response.Content.Single(x => x.ProductId == product1Id);
            Assert.AreEqual("invoice_123", discountItem.InvoiceId);
            Assert.AreEqual("ATOC", discountItem.Vendor);
            Assert.AreEqual(-15, discountItem.Amount);

            discountItem = response.Content.Single(x => x.ProductId == product2Id);
            Assert.AreEqual("invoice_123", discountItem.InvoiceId);
            Assert.AreEqual("ATOC", discountItem.Vendor);
            Assert.AreEqual(-15, discountItem.Amount);
        }
        
        [Test]
        public async Task Apply_Throws_PromocodeCurrencyNotApplicable_When_I_Pass_An_Invalid_Currency()
        {
            var voucher = Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "DKK",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = "Product_123",
                            ProductUri = new Uri("https://product.com/Product_123"),
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);


            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.PromocodeCurrencyNotApplicable, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_When_I_Pass_A_Travel_Voucher_On_A_RailcardOrder()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var discountCard = Given.ADiscountCard("P1Y");
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 10,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = "Product_123",
                            ProductUri = new Uri("https://product.com/Product_123"),
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.DiscountCardProtocolV1MediaType);
            Given.ADiscountCardDetailsForRailcardInInvoice(invoices, discountCard);
            Given.ADiscountCardCodeForRailcardInInvoice(new Uri(discountCard.CardDetails.CardType.Url), InMemoryCardTypeClient.AvantageAdultCode);
            Given.ACodeInvalidForProductType(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ValidationWillFail(voucher.Code, new NotApplicableException(ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix));

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");
           
            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_When_I_Pass_A_Railcard_Voucher_On_A_TraveldOrder()
        {
            var voucher = Given.AVoucherForRailcardWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var productId = "Product_456";
            var productUri = new Uri($"https://product.com/{productId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 10,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_456",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.ACodeInvalidForProductType(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ValidationWillFail(voucher.Code, new NotApplicableException(ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix));
            Given.ATravelProtocolProduct(productUri);

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_When_I_Pass_A_Network_Railcard_Voucher_On_A_RailcardOrder_That_Contains_Other_Railcard()
        {
            var voucher = Given.AVoucherForRailcardWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var productId = "Product_456";
            var productUri = new Uri($"https://product.com/{productId}");
            var discountCard = Given.ADiscountCard("P1Y");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 10,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_456",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.DiscountCardProtocolV1MediaType);
            Given.ADiscountCardDetailsForRailcardInInvoice(invoices, discountCard);
            Given.ADiscountCardCodeForRailcardInInvoice(new Uri(discountCard.CardDetails.CardType.Url), InMemoryCardTypeClient.AvantageAdultCode);
            Given.ACodeInvalidForSpecificRailcard(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ValidationWillFail(voucher.Code, new NotApplicableException(ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix));

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_When_I_Pass_A_CarrierCode_That_Is_Not_Valid_For_Campaign()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var productId = "Product_456";
            var productUri = new Uri($"https://product.com/{productId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 10,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_456",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AVoucherNotValidForCarrierCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(productUri, carrierCode : "FakeCode");
            Given.ValidationWillFail(voucher.Code, new NotApplicableException(ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix));

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_When_I_Pass_A_TickeTtype_That_Is_Not_Valid_For_Campaign()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var productId = "Product_456";
            var productUri = new Uri($"https://product.com/{productId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 10,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_456",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AVoucherNotValidForCarrierCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(productUri, fareTypeCode: "fakeCode");
            Given.ValidationWillFail(voucher.Code, new NotApplicableException(ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix));

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_WhenCustomerHasIsNewAttributeOfTrue_VoucherifyApplyIsCalledWithCustomerIsNew()
        {
            var voucherCode = Guid.NewGuid().ToString();
            var voucher = Given.AVoucherWithCode(voucherCode);
            var customerId = Guid.NewGuid().ToString();
            var newCustomerUri = new Uri($"http://customer/{customerId}");
            var invoices = new Invoice[] { Invoice() };

            Given.ACustomerThatIsDefinedAsANewCustomer(newCustomerUri, new ExternalServices.Customer.Contract.Customer { Id = customerId });
            await Given.APromocode(Promocode(voucherCode));
            Given.AValidatedWithCode(voucherCode, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(invoices.First().ProductItems.First().ProductUri);

            await When.IMake<Contract.DiscountItem[]>(AnApplyRequest(invoices).WithHeaders(("CustomerUri", newCustomerUri.ToString())));

            var newCustomer = "newCustomer";
            var validationRequestsWithNewCustomer = Voucherify.ValidationRequests.Where(r => r.VoucherCode == voucherCode && r.Validation.Customer.metadata.isnew_status == newCustomer);
            Assert.IsTrue(validationRequestsWithNewCustomer.Any(), CustomerCategoryFailMessage(voucherCode, newCustomer));
        }

        [Test]
        public async Task Apply_WhenCustomerHasIsNewAttributeOfFalse_VoucherifyApplyIsCalledWithCustomerIsRepeatCustomer()
        {
            var voucherCode = Guid.NewGuid().ToString();
            var voucher = Given.AVoucherWithCode(voucherCode);
            var customerId = Guid.NewGuid().ToString();
            var repeatCustomerUri = new Uri($"http://customer/{customerId}");
            var invoices = new Invoice[] { Invoice() };

            Given.ACustomerThatIsDefinedAsANotNewCustomer(repeatCustomerUri, new ExternalServices.Customer.Contract.Customer { Id = customerId });
            await Given.APromocode(Promocode(voucherCode));
            Given.AValidatedWithCode(voucherCode, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(invoices.First().ProductItems.First().ProductUri);

            await When.IMake<Contract.DiscountItem[]>(AnApplyRequest(invoices).WithHeaders(("CustomerUri", repeatCustomerUri.ToString())));

            var repeatCustomer = "repeatCustomer";
            var validationRequestsWithRepeatCustomer = Voucherify.ValidationRequests.Where(r => r.VoucherCode == voucherCode && r.Validation.Customer.metadata.isnew_status == repeatCustomer);
            Assert.IsTrue(validationRequestsWithRepeatCustomer.Any(), CustomerCategoryFailMessage(voucherCode, repeatCustomer));
        }


        [Test]
        public async Task Apply_WhenCustomerInformationIsNotSupplied_VoucherifyApplyIsCalledWithCustomerNotIdentified()
        {
            var voucherCode = Guid.NewGuid().ToString();
            var voucher = Given.AVoucherWithCode(voucherCode);
            var customerId = Guid.NewGuid().ToString();
            var invoices = new Invoice[] { Invoice() };

            await Given.APromocode(Promocode(voucherCode));
            Given.AValidatedWithCode(voucherCode, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ATravelProtocolProduct(invoices.First().ProductItems.First().ProductUri);

            var response = await When.IMake<Contract.DiscountItem[]>(AnApplyRequest(invoices));

            var customerNotIdentified = "customerNotIdentified";
            var validationRequestsWithCustomerNotIdentified = Voucherify.ValidationRequests.Where(r => r.VoucherCode == voucherCode && r.Validation.Customer.metadata.isnew_status == customerNotIdentified);
            Assert.IsTrue(validationRequestsWithCustomerNotIdentified.Any(), CustomerCategoryFailMessage(voucherCode, customerNotIdentified));
        }

        [Test]
        public async Task Apply_WhenCustomerInformationIsNotSuppliedAndVoucherifyReturnsCustomerNotNewValidationError_ErrorWithCodeCustomerNotNewIsReturned()
        {
            var voucherCode = Guid.NewGuid().ToString();
            var voucher = Given.AVoucherWithCode(voucherCode);
            var customerId = Guid.NewGuid().ToString();
            var invoices = new Invoice[] { Invoice() };

            await Given.APromocode(Promocode(voucherCode));
            Given.ValidationWillFail(voucherCode, new NotApplicableException("VoucherifyCustomerNotNew_PromocodeCustomerNotNew"));
            Given.ATravelProtocolProduct(invoices.First().ProductItems.First().ProductUri);

            var response = await When.IMake<NetStandard.Exceptions.Contracts.ErrorResponse>(AnApplyRequest(invoices));

            var promoCodeCustomerNotNewCode = "41708.25";
            var promoCodeCustomerNotNew = "PromocodeCustomerNotNew";
            var promoCodeCustomerNotNewErrors = response.Content.Errors.Where(e => e.Code == promoCodeCustomerNotNewCode && e.Detail == promoCodeCustomerNotNew);
            Assert.IsTrue(promoCodeCustomerNotNewErrors.Any());
        }

        [Test]
        public async Task Apply_WhenCustomerIsNotNewAndTriesToApplyAVoucherForNewCustomersOnly_Throws_RedemptionTotalLimitReached()
        {
            var voucher = Given.AVoucherWithCode(Code);
            var customerId = Guid.NewGuid().ToString();
            var repeatCustomerUri = new Uri($"http://customer/{customerId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = "Product_123",
                            ProductUri = new Uri("https://product.com/Product_123"),
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.ACustomerThatIsDefinedAsANotNewCustomer(repeatCustomerUri, new ExternalServices.Customer.Contract.Customer { Id = customerId });
            Given.ValidationWillFail(voucher.Code, new NotApplicableException("VoucherifyCustomerNotNew_PromocodeCustomerNotNew"));
            Given.ATravelProtocolProduct(invoices.First().ProductItems.First().ProductUri);

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.PromocodeCustomerNotNew, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_PromocodeExpiredException_When_I_Pass_An_Expired_Promocode()
        {
            var voucher = Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(-2),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = "Product_123",
                            ProductUri = new Uri("https://product.com/Product_123"),
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);


            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.PromocodeExpired, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task Apply_Throws_InvalidPromocode_When_I_Pass_An_Inactive_Promocode()
        {
            var voucher = Given.AnInactiveVoucherWithCode(Code);
            var productId = "Product_123";
            var productUri = new Uri($"https://product.com/{productId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AnInvalidWithInactiveCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ValidationWillFail(voucher.Code, new InvalidPromocodeException("Promocode is inactive."));
            Given.ATravelProtocolProduct(productUri);

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.InvalidPromocode, response.Content.Errors.FirstOrDefault()?.Code);
        }

        private string CustomerCategoryFailMessage(string voucherCode, string expectedCustomerCategory)
        {
            return $"Expected a validation request of '{expectedCustomerCategory}' but found {string.Join(",", Voucherify.ValidationRequests.Where(r => r.VoucherCode == voucherCode).Select(r => $"'{r.Validation.Customer.metadata.isnew_status}'"))}";
        }

        private Request<IEnumerable<Invoice>> AnApplyRequest(IEnumerable<Invoice> invoices)
        {
            return new Request<IEnumerable<Invoice>>
            {
                Content = invoices,
                Location = $"/promocodes/{PromocodeId}/apply",
                Method = HttpMethod.Post
            };
        }

        public Service.Repository.Entities.Promocode Promocode(string voucherCode)
        {
            return new Service.Repository.Entities.Promocode()
            {
                Code = voucherCode,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId
            };
        }

        private static Invoice Invoice()
        {
            return new Invoice
            {
                Id = "invoice_123",
                CurrencyCode = "GBP",
                ProductItems = new List<Item>
                {
                    new Item
                    {
                        Amount = 100,
                        ProductId = "Product_123",
                        ProductUri = new Uri("https://product.com/Product_123"),
                        Vendor = "ATOC"
                    }
                }
            };
        }

        [Test]
        public async Task Apply_Throws_WithMaxedOutReedemedVouchersInCampaign_Throws_RedemptionTotalLimitReached()
        {
            var voucher = Given.AVoucherWithCode(Code);

            var campaign = await Given.AnInvalidValidCampaign(voucher.CampaignId);
            var productId = "Product_123";
            var productUri = new Uri($"https://product.com/{productId}");

            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId,
                CampaignId = campaign.CampaignId,
                ValidationRuleId = "notExistingRuleId"
            });
            var invoices = new Invoice[]
            {
                new Invoice
                {
                    Id = "invoice_123",
                    CurrencyCode = "GBP",
                    ProductItems = new List<Item>
                    {
                        new Item
                        {
                            Amount = 100,
                            ProductId = productId,
                            ProductUri = productUri,
                            Vendor = "ATOC"
                        }
                    }
                }
            };
            Given.AnInvaliddWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);
            Given.ValidationWillFail(voucher.Code, new RedemptionTotalLimitReachedException("Campaign has reached maximum number of vouchers redeemed."));
            Given.ATravelProtocolProduct(productUri);

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{PromocodeId}/apply");
           
            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.RedemptionTotalLimitReached, response.Content.Errors.FirstOrDefault()?.Code);
        }
    }
}
