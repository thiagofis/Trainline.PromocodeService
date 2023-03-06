using AutoFixture;
using AutoFixture.Dsl;
using System;
using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.Contract;

namespace Trainline.PromocodeService.Acceptance.Helpers.Steps
{
    public class GivenSteps
    {
        protected readonly Fixture _fixture;

        public GivenSteps(Fixture fixture)
        {
            _fixture = fixture;
        }

        public TRequest AValid<TRequest>()
        {
            return _fixture.Create<TRequest>();
        }

        public ICustomizationComposer<TRequest> A<TRequest>()
        {
            return _fixture.Build<TRequest>();
        }

        public Invoice[] Invoices() => new Invoice[]
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
                    },
                    new Item
                    {
                        Amount = 50,
                        ProductId = "Product_456",
                        ProductUri = new Uri("https://product.com/Product_456"),
                        Vendor = "ATOC"
                    }
                }
            }
        };

        public QuoteRequest[] QuoteRequestsForAnInvoice(Invoice invoice)
        {
            var product = invoice.ProductItems.First();

            return Enumerable.Range(1, 2).Select(i => new QuoteRequest
            {
                ReferenceId = $"{product.ProductId}-{i}r",
                ProductUri = product.ProductUri,
                RefundableAmount = new Money
                {
                    Amount = Math.Round(product.Amount / 3, 2),
                    CurrencyCode = invoice.CurrencyCode
                },
                ListedAmount = new Money
                {
                    Amount = Math.Round(product.Amount / 3, 2),
                    CurrencyCode = invoice.CurrencyCode
                },
            }).ToArray();
        }
    }
}
