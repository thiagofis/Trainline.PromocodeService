using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.DiscountStrategies;
using Trainline.PromocodeService.Service.Mappers;
using Discount = Trainline.PromocodeService.ExternalServices.Voucherify.Contract.Discount;

namespace Trainline.PromocodeService.Service
{
    public interface IInvoiceGenerator
    {
        IEnumerable<DiscountInvoiceInfo> Generate(IVoucherifyResponse response, ICollection<InvoiceInfo> invoices);
    }

    public class InvoiceGenerator : IInvoiceGenerator
    {
        public const string TiersEnableMetadataKey = "TiersEnabled";
        public const string TierTwoDiscountMetadataKey = "Tier_Discount";
        public const string TierTwoThresholdMetadataKey = "Tier_Threshold";

        private readonly IPromocodeMapper _promocodeMapper;
        private readonly IPromocodeDiscountStrategyFactory _discountStrategyFactory;

        public InvoiceGenerator(IPromocodeMapper promocodeMapper, IPromocodeDiscountStrategyFactory discountStrategyFactory)
        {
            _promocodeMapper = promocodeMapper;
            _discountStrategyFactory = discountStrategyFactory;
        }

        public IEnumerable<DiscountInvoiceInfo> Generate(IVoucherifyResponse response, ICollection<InvoiceInfo> invoices)
        {
            var resultInvoices = invoices.ToDictionary(k => k.Id, v => new DiscountInvoiceInfo
            {
                CurrencyCode = v.CurrencyCode,
                Id = v.Id,
                Items = new List<DiscountItem>()
            });

            var productIdToInvoiceIdLookup = GetProductIdInvoiceNumberLookup(invoices);
            
            var productsToDiscount = response.ApplicableTo?.Data.Select(x => x.SourceId).ToArray() ?? new string[0];
            var lineItemsToDiscount = invoices.SelectMany(x => x.ProductItems)
                .Where(x => productsToDiscount.Contains(x.ProductId))
                .ToList();

            var discount = GetDiscount(response, lineItemsToDiscount);
            var strategy = _discountStrategyFactory.Get(discount);
            var discountLineItems = strategy.GenerateDiscountItems(discount, lineItemsToDiscount);

            foreach (var discountLineItem in discountLineItems)
            {
                var invoiceId = productIdToInvoiceIdLookup[discountLineItem.ProductId];
                resultInvoices[invoiceId].Items.Add(discountLineItem);
            }

            return resultInvoices.Select(x => x.Value);
        }

        private Dictionary<string, string> GetProductIdInvoiceNumberLookup(ICollection<InvoiceInfo> invoices)
        {
            var resultLookup = new Dictionary<string, string>();
            foreach (var invoice in invoices)
            {
                foreach (var invoiceLineItem in invoice.ProductItems)
                {
                    resultLookup.Add(invoiceLineItem.ProductId, invoice.Id);
                }
            }
            return resultLookup;
        }

        private Model.Discount GetDiscount(IVoucherifyResponse response, IEnumerable<ProductItem> productItems)
        {
            var discount = response.Discount;
            if (response.Metadata != null && response.Metadata.TryGetValue(TiersEnableMetadataKey, out var value)
                                          && bool.TryParse(value, out var isTiersEnable)
                                          && isTiersEnable)
            {
                if (!response.Metadata.ContainsKey(TierTwoDiscountMetadataKey) ||
                    !response.Metadata.ContainsKey(TierTwoThresholdMetadataKey))
                {
                    throw new InvalidOperationException($"Invalid metadata configuration. Whenever {TiersEnableMetadataKey} is {TierTwoDiscountMetadataKey} and {TierTwoThresholdMetadataKey} are required");
                }

                if (!decimal.TryParse(response.Metadata[TierTwoDiscountMetadataKey], out var tierTwoDiscount) ||
                    !decimal.TryParse(response.Metadata[TierTwoThresholdMetadataKey], out var tierTwoThreshold))
                {
                    throw new InvalidOperationException($"Value of {TierTwoDiscountMetadataKey} and {TierTwoThresholdMetadataKey} should be numeric");
                }

                var totalPriceOfApplicableProducts = productItems.Sum(x => x.Amount);

                if (totalPriceOfApplicableProducts >= tierTwoThreshold)
                {
                    discount = new Discount
                    {
                        Type = response.Discount.Type,
                        AmountOff = tierTwoDiscount.ToVoucherifyPrice()
                    };
                }
            }

            return _promocodeMapper.Map(discount);
        }
    }
}
