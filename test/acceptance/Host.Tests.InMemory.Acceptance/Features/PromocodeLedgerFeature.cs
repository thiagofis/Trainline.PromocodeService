using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{

    [TestFixture]
    public class PromocodeLedgerFeature : AcceptanceTestBase
    {
        private const string Code = "testCode123";

        [Test]
        public async Task LedgerIsCreatedAfterRedemption()
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

            var redemptionResponse = await When.IPost<Invoice[], Contract.Redeemed>(invoices, $"/promocodes/{promocode.PromocodeId}/redeem");
            Then.TheResponseCodeShouldBe(redemptionResponse, HttpStatusCode.OK);
            var ledgerLink = redemptionResponse.Content.Links["ledger"].Href;

            var ledgerResponse = await When.IGet<Contract.Ledger>(ledgerLink);
            var ledger = ledgerResponse.Content;
            foreach (var item in redemptionResponse.Content.DiscountItems)
            {
                Assert.AreEqual(ledger.Lines.Single(x => x.ProductUri == item.ProductUri).Amount, item.Amount);
            }

            foreach (var item in invoices.SelectMany(x => x.ProductItems))
            {
                Assert.AreEqual(ledger.Products.Single(x => x.ProductUri == item.ProductUri).ProductPrice, item.Amount);
            }
        }

        [Test]
        public async Task CanCreateAndForfeitQuoteForRedeemedVoucher()
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

            var quoteRequests = Given.QuoteRequestsForAnInvoice(invoices[0]);

            var redemptionResponse = await When.IPost<Invoice[], Contract.Redeemed>(invoices, $"/promocodes/{promocode.PromocodeId}/redeem");
            Then.TheResponseCodeShouldBe(redemptionResponse, HttpStatusCode.OK);

            var quoteUri = redemptionResponse.Content.Links["quotes"].Href;

            var ledgerLink = redemptionResponse.Content.Links["ledger"].Href;
            var ledgerResponse = await When.IGet<Contract.Ledger>(ledgerLink);
            Then.TheResponseCodeShouldBe(ledgerResponse, HttpStatusCode.OK);

            var expectedLinesInLedgerAfterForfeit = ledgerResponse.Content.Lines.Count + 1;

            var quoteResponse = await When.IPost<Contract.QuoteRequest[], List<Contract.PromoQuote>>(quoteRequests,
                quoteUri);
            Then.TheResponseCodeShouldBe(quoteResponse, HttpStatusCode.OK);

            var forfeitLink = quoteResponse.Content[0].Links["forfeit"].Href;
            var forfeitResponse = await When.IPost<object>(forfeitLink);
            Then.TheResponseCodeShouldBe(forfeitResponse, HttpStatusCode.OK);

            var ledgerResponse2 = await When.IGet<Contract.Ledger>(ledgerLink);
            Then.TheResponseCodeShouldBe(ledgerResponse2, HttpStatusCode.OK);

            Assert.AreEqual(expectedLinesInLedgerAfterForfeit, ledgerResponse2.Content.Lines.Count);
        }

        [Test]
        public async Task CreateQuote_EquivalentQuoteAlreadyExists_ReturnsExistingQuote()
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

            var quoteRequest = Given.QuoteRequestsForAnInvoice(invoices[0]).Take(1).ToArray();

            var redemptionResponse = await When.IPost<Invoice[], Contract.Redeemed>(invoices, $"/promocodes/{promocode.PromocodeId}/redeem");
            Then.TheResponseCodeShouldBe(redemptionResponse, HttpStatusCode.OK);

            var quoteUri = redemptionResponse.Content.Links["quotes"].Href;

            var quoteResponse = await When.IPost<Contract.QuoteRequest[], List<Contract.PromoQuote>>(quoteRequest,
                quoteUri);
            Then.TheResponseCodeShouldBe(quoteResponse, HttpStatusCode.OK);
            var quote1 = quoteResponse.Content.Single();

            var quoteResponse2 = await When.IPost<Contract.QuoteRequest[], List<Contract.PromoQuote>>(quoteRequest,
                quoteUri);
            Then.TheResponseCodeShouldBe(quoteResponse2, HttpStatusCode.OK);
            var quote2 = quoteResponse2.Content.Single();

            Assert.AreEqual(quote1.Id, quote2.Id);
        }
    }
}
