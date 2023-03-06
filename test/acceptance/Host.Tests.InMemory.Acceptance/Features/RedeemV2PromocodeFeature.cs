using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Acceptance.Helpers.TestContracts;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{
    [TestFixture]
    public class RedeemV2PromocodeFeature : AcceptanceTestBase
    {
        private RequestOptions _options;
        private const string Code = "testCode123";

        [SetUp]
        public void Setup()
        {
            _options = new RequestOptions
            {
                Accept = MediaTypes.PromocodeV2
            };
        }

        [Test]
        public async Task RedeemPromocode_Valid()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var promocode = await Given.APromocodeFromVoucher(voucher);
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

            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AValidRedemptionForInvoices(Code, invoices);
            Given.ATravelProtocolProduct(product1Uri);
            Given.ATravelProtocolProduct(product2Uri);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Contract.Redeemed>(redeemPromocode
                , $"/promocodes/{promocode.PromocodeId}/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.OK);

            foreach (var item in invoices.SelectMany(x => x.ProductItems))
            {
                var discountItem = response.Content.DiscountItems.Single(x => x.ProductUri == item.ProductUri);
                Assert.AreEqual(item.ProductId, discountItem.ProductId);
                Assert.AreEqual(invoices[0].Id, discountItem.InvoiceId);
            }
        }

        [Test]
        public async Task RedeemPromocode_ValidWithTier()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1, tierDiscountAmount: 30m, tierThreshold: 150);
            var promocode = await Given.APromocodeFromVoucher(voucher);
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

            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AValidRedemptionForInvoices(Code, invoices);
            Given.ATravelProtocolProduct(product1Uri);
            Given.ATravelProtocolProduct(product2Uri);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Contract.Redeemed>(redeemPromocode
                , $"/promocodes/{promocode.PromocodeId}/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.OK);
            var items = invoices.SelectMany(x => x.ProductItems).ToList();
            foreach (var item in items)
            {
                var discountItem = response.Content.DiscountItems.Single(x => x.ProductUri == item.ProductUri);
                Assert.AreEqual(item.ProductId, discountItem.ProductId);
                Assert.AreEqual(invoices[0].Id, discountItem.InvoiceId);
            }
            Assert.AreEqual(-30m, response.Content.DiscountItems.Sum(i => i.Amount));
        }

        [Test]
        public async Task RedeemPromocode_InvalidVoucher()
        {
            var code = "testvoucher";
            var voucher = Given.AVoucherWithCode(code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = "testvoucher",
                ValidityStartDate = DateTime.Now.AddDays(+10),
                ValidityEndDate = DateTime.Now.AddDays(-9),
                CurrencyCode = "GBP2",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 10,
                RedemptionQuantity = 1,
                PromocodeId = "TestPromocodeId",
                ValidationRuleId = "notExistingRuleId"
            });

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = Array.Empty<Invoice>(),
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/TestPromocodeId/redeem", _options);
            
            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);
            Assert.AreEqual(4, response.Content.Errors.Count);

            var errorCodes = response.Content.Errors.Select(x => x.Code).ToArray();
            Assert.Contains(ErrorCodes.PromocodeHaveNotStarted, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeExpired, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeAlreadyRedeemed, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeCurrencyNotApplicable, errorCodes);
        }

        [Test]
        public async Task RedeemPromocode_VoucherNotExists()
        {
            var code = "testvoucher";

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = Array.Empty<Invoice>(),
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/{code}/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);
            Assert.AreEqual(1, response.Content.Errors.Count);

            Assert.AreEqual(ErrorCodes.PromocodeNotFound, response.Content.Errors[0].Code);
        }

        [Test]
        public async Task RedeemPromocode_Throws_PromocodeCurrencyNotApplicable_When_I_Pass_An_Invalid_Currency()
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
                PromocodeId = "PromocodeId123",
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
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.DiscountCardProtocolV1MediaType);
            Given.AValidatedWithCode(Code, invoices.SelectMany(x => x.ProductItems), 10);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/PromocodeId123/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.PromocodeCurrencyNotApplicable, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task RedeemPromocode_Throws_WithMaxedOutReedemedVouchersInCampaign_Throws_RedemptionTotalLimitReached()
        {
            var voucher = Given.AVoucherWithCode(Code);
            var promocode = await Given.APromocodeFromVoucher(voucher);
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
                PromocodeId = "PromocodeId123",
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
            Given.AProtocolForProductsInInvoice(invoices, InMemorySupportedProtocolsService.TravelProtocolV3MediaType);
            Given.AnInvalidRedemptionForInvoices(Code, invoices);
            Given.ATravelProtocolProduct(productUri);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/PromocodeId123/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.RedemptionTotalLimitReached, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task RedeemPromocode_Throws_WhenIPassATravelProductForARailCardVoucher()
        {
            var voucher = Given.AVoucherForRailcardWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var promocode = await Given.APromocodeFromVoucher(voucher);
            var productId = "Product_456";
            var productUri = new Uri($"https://product.com/{productId}");

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
            Given.ATravelProtocolProduct(productUri);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/{promocode.PromocodeId}/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task RedeemPromocode_Throws_WhenIPassARailcardProductForATravelVoucher()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var promocode = await Given.APromocodeFromVoucher(voucher);
            var discountCard = Given.ADiscountCard("P1Y");

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
            Given.ADiscountCardCodeForRailcardInInvoice(new Uri(discountCard.CardDetails.CardType.Url), InMemoryCardTypeClient.Code);

            var redeemPromocode = new RedeemPromocode
            {
                Invoices = invoices,
                RetentionDate = null,
            };

            var response = await When.IPost<RedeemPromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(redeemPromocode
                , $"/promocodes/{promocode.PromocodeId}/redeem", _options);

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }

        [Test]
        public async Task RedeemPromocode_Throws_When_I_Pass_A_Network_Railcard_Voucher_On_A_RailcardOrder_That_Contains_Other_Railcard()
        {
            var voucher = Given.AVoucherWithCode(Code, redeemedCount: 0, redemptionCount: 1);
            var promocode = await Given.APromocodeFromVoucher(voucher);
            var discountCard = Given.ADiscountCard("P1Y");

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

            var response = await When.IPost<Invoice[], Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(invoices, $"/promocodes/{promocode.PromocodeId}/redeem");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.ExcludedProductTypeMatched, response.Content.Errors.FirstOrDefault()?.Code);
        }
    }
}
