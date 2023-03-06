using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Contract;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Features
{
    public class LedgerFeature : AcceptanceTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Context.ContextUri = ContextUriCreator.Create(AppSettings.ContextUri);
        }
        
        [TestCase(true, -1)]
        [TestCase(false, 0)]
        public async Task GivenARedeemedPromocode_WhenIQuoteAndForfeit_ThenWeDeductedExpectedAmountsd(bool withListedAmount, decimal expectedAmount)
        {
            var productUri = await DummyProductCreator.Create(AppSettings.DummyInventoryUri, Context.ContextUri);
            var redeemed = await GivenRedeemedItems(AppSettings.TestPromocode, productUri);

            await WhenQuoteAndForfeit(redeemed.QuoteUri(), productUri, 5, withListedAmount);

            await ThenTheRemainingPromoForProductIs(redeemed.LedgerUri(), productUri, expectedAmount);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public async Task GivenARedeemedPromocode_WhenIQuoteAndForfeit_ThenAllOfThePromoShouldBeDeducted(bool withListedAmount)
        {
            var productUri = await DummyProductCreator.Create(AppSettings.DummyInventoryUri, Context.ContextUri);
            var redeemed = await GivenRedeemedItems(AppSettings.TestPromocode, productUri);

            await WhenQuoteAndForfeit(redeemed.QuoteUri(), productUri, 5, withListedAmount);
            await WhenQuoteAndForfeit(redeemed.QuoteUri(), productUri, 5, withListedAmount);

            await ThenTheRemainingPromoForProductIs(redeemed.LedgerUri(), productUri, 0m);
        }
        
        private async Task WhenQuoteAndForfeit(Uri quoteUri, Uri productUri, decimal amount, bool includeListAmount = true)
        {
            var result = await When.IPost<QuoteRequest[], Contract.PromoQuote[]>(
                new QuoteRequest[]
                {
                    new QuoteRequest
                    {
                        ReferenceId = Guid.NewGuid().ToString(),
                        ProductUri = productUri,
                        ListedAmount = includeListAmount ? new Money
                        {
                            Amount = amount,
                            CurrencyCode = "GBP"
                        } : null,
                        RefundableAmount = new Money
                        {
                            Amount = amount,
                            CurrencyCode = "GBP"
                        },
                    }
                }, quoteUri);

            var forfeitUris = result.Content.SelectMany(x => x.Links)
                .Where(l => l.Key == "forfeit")
                .Select(x => x.Value.Href);

            await Task.WhenAll(forfeitUris.Select(x =>  When.IPost<string>(x)));
        }

        private async Task<Contract.Redeemed> GivenRedeemedItems(string code, params Uri[] productUris)
        {
            var promocode = await GivenICreatePromocode(code);
            var redeemed = await GivenRedeemedPromoWithProducts(promocode, productUris);
            return redeemed;
        }

        private async Task<Contract.Promocode> GivenICreatePromocode(string code)
        {
            var result = await When.IPost<CreatePromocode, Contract.Promocode>(new CreatePromocode
            {
                Code = code
            }, "/promocodes/");
            return result.Content;
        }

        private async Task<Contract.Redeemed> GivenRedeemedPromoWithProducts(Contract.Promocode promocode, params Uri[] productUris)
        {
            var response = (await When.IPost<Invoice[], Contract.Redeemed>(new Invoice[]
            {
                new Invoice
                {
                    Id = Guid.NewGuid().ToString(),
                    CurrencyCode = "GBP",
                    ProductItems = productUris.Select(x => new Item
                    {
                        ProductId = x.ToString(),
                        ProductUri = x,
                        Vendor = "ATOC",
                        Amount = 10
                    }).ToList()
                }
            }, $"/promocodes/{promocode.PromocodeId}/redeem"));
            return response.Content;
        }

        private async Task ThenTheRemainingPromoForProductIs(Uri legerUri, Uri productUri, decimal expectedAmount)
        {
            var leger = (await When.IGet<Contract.Ledger>(legerUri)).Content;

            var remainingPromo = leger
                .Lines
                .Where(x => x.ProductUri == productUri)
                .Sum(x => x.Amount);

            Assert.AreEqual(expectedAmount, remainingPromo);
        }
    }
}
