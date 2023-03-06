using System;
using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Host.UnitTests.Builders
{
    public class InvoiceBuilder
    {
        private readonly InvoiceInfo _invoiceInfo;

        public InvoiceBuilder(InvoiceInfo invoiceInfo)
        {
            this._invoiceInfo = invoiceInfo;
        }

        public static InvoiceBuilder Invoice(string id = "InvoiceId_123", string currencyCode = "GBP")
        {
            return new InvoiceBuilder(new InvoiceInfo()
            {
                Id = id,
                CurrencyCode = currencyCode,
                ProductItems = new List<ProductItem>()
            });
        }

        public InvoiceBuilder WithLineItem(string productId, decimal amount, Uri productUri)
        {
            this._invoiceInfo.ProductItems.Add(
                new ProductItem
                {
                    Amount = amount,
                    ProductId = productId,
                    ProductUri = productUri
                });

            return this;
        }

        public static implicit operator InvoiceInfo(InvoiceBuilder builder)
        {
            return builder._invoiceInfo;
        }
    }
}
