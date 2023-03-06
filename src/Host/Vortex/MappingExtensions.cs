using System;
using System.Collections.Generic;
using System.Linq;

namespace Trainline.PromocodeService.Host.Vortex
{
    public static class MappingExtensions
    {
        private const string MinimumAmount = "0";
        public static PromocodeCreated ToCreatedVortexEvent(this Model.Promocode promocode)
        {
            var orderMinimumAmount = promocode.ValidationRules.FirstOrDefault(x => x.Name == "OrderMinimumSpend")?.Value;

            return new PromocodeCreated
            {
                Code = promocode.Code,
                PromocodeId = promocode.PromocodeId,
                ValidityStartDate = new DateTimeOffset(promocode.ValidityStartDate),
                ValidityEndDate = new DateTimeOffset(promocode.ValidityEndDate),
                RedemptionQuantity = promocode.RedemptionQuantity ?? int.MaxValue,
                RedeemedQuantity = promocode.RedeemedQuantity,
                CurrencyCode = promocode.CurrencyCode,
                OrderMinimumAmount = orderMinimumAmount ?? MinimumAmount,
                Discount = new Discount
                {
                    Type = promocode.Discount.Type,
                    Amount = promocode.Discount.Amount
                },
                CampaignName = promocode.CampaignName ?? string.Empty
            };
        }

        public static PromocodeValidated ToValidatedVortexEvent(this IEnumerable<Model.DiscountInvoiceInfo> invoiceInfos, string promocodeId, string campaignName)
        {
            var discountItems = invoiceInfos
                .SelectMany(i =>
                    i.Items.Select(pi => new DiscountItem
                    {
                        ProductId = pi.ProductId,
                        ProductUri = pi.ProductUri,
                        Vendor = pi.Vendor,
                        Amount = pi.Amount,
                        InvoiceId = i.Id,
                        CurrencyCode = i.CurrencyCode
                    }))
                .ToList();

            return new PromocodeValidated
            {
                PromocodeId = promocodeId,
                DiscountItems = discountItems,
                CampaignName = campaignName ?? string.Empty
            };
        }

        public static PromocodeRedeemed ToRedeemedVortexEvent(this ICollection<Model.DiscountInvoiceInfo> invoiceInfos, string promocodeId, string campaignName)
        {
            var discountItems = invoiceInfos
                .SelectMany(i =>
                    i.Items.Select(pi => new DiscountItem
                    {
                        ProductId = pi.ProductId,
                        ProductUri = pi.ProductUri,
                        Vendor = pi.Vendor,
                        Amount = pi.Amount,
                        InvoiceId = i.Id,
                        CurrencyCode = i.CurrencyCode
                    }))
                .ToList();

            return new PromocodeRedeemed
            {
                PromocodeId = promocodeId,
                DiscountItems = discountItems,
                CampaignName = campaignName ?? string.Empty
            };
        }
    }
}
