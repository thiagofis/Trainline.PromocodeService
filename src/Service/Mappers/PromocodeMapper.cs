using System;
using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Model;
using Discount = Trainline.PromocodeService.Model.Discount;

namespace Trainline.PromocodeService.Service.Mappers
{

    public class PromocodeMapper : IPromocodeMapper
    {
        private readonly IValidationRuleMapper _validationRuleMapper;

        public PromocodeMapper(IValidationRuleMapper validationRuleMapper)
        {
            _validationRuleMapper = validationRuleMapper;
        }

        public Repository.Entities.Promocode Map(Voucher voucher)
        {
            return new Repository.Entities.Promocode
            {
                Code = voucher.Code,
                ValidityStartDate = voucher.StartDate,
                ValidityEndDate = voucher.ExpirationDate,
                DiscountType = voucher.Discount?.Type,
                DiscountAmount = GetAmount(voucher.Discount),
                RedeemedQuantity = voucher.Redemption.RedeemedQuantity,
                RedemptionQuantity = voucher.Redemption.Quantity,
                CurrencyCode = voucher.Metadata[MetadataKeys.CurrencyCode].ToString(),
                ValidationRuleId = voucher.ValidationRulesAssignments?.Data?.FirstOrDefault()?.RuleId,
                CampaignId =  voucher.CampaignId,
                CampaignName = voucher.Campaign,
                ProductType = voucher.Metadata.TryGetValue(MetadataKeys.ProductType, out var productType) ? productType : "travel"
            };
        }

        public Promocode Map(Repository.Entities.Promocode promocodeEntity, IEnumerable<Repository.Entities.ValidationRule> validationRuleEntities)
        {
            return new Promocode
            {
                Id = promocodeEntity.Id,
                Code = promocodeEntity.Code,
                ValidityStartDate = DateTime.SpecifyKind(promocodeEntity.ValidityStartDate, DateTimeKind.Utc),
                ValidityEndDate = DateTime.SpecifyKind(promocodeEntity.ValidityEndDate, DateTimeKind.Utc),
                CurrencyCode = promocodeEntity.CurrencyCode,
                RedeemedQuantity = promocodeEntity.RedeemedQuantity,
                RedemptionQuantity = promocodeEntity.RedemptionQuantity,
                Discount = new Discount
                {
                    Type = promocodeEntity.DiscountType,
                    Amount = promocodeEntity.DiscountAmount
                },
                ValidationRules = _validationRuleMapper.Map(promocodeEntity, validationRuleEntities),
                PromocodeId = promocodeEntity.PromocodeId,
                State = SetState(promocodeEntity),
                ProductType = promocodeEntity.ProductType,
                CampaignName = promocodeEntity.CampaignName
            };
        }

        private PromocodeState SetState(Repository.Entities.Promocode promocodeEntity)
        {
            if (promocodeEntity.RedemptionQuantity != null &&
                promocodeEntity.RedeemedQuantity >= promocodeEntity.RedemptionQuantity)
                return PromocodeState.Redeemed;
            if (promocodeEntity.ValidityStartDate > DateTimeOffset.UtcNow)
                return PromocodeState.NotStarted;
            if (promocodeEntity.ValidityEndDate < DateTimeOffset.UtcNow)
                return PromocodeState.Expired;
            return PromocodeState.Open;
        }

        public Discount Map(ExternalServices.Voucherify.Contract.Discount discount)
        {
            return new Discount
            {
                Type = discount.Type,
                Amount = GetAmount(discount)
            };
        }

        private decimal GetAmount(ExternalServices.Voucherify.Contract.Discount discount)
        {
            switch (discount.Type)
            {
                case DiscountTypeDefinitions.Amount:
                    return discount.AmountOff.ToPromoPrice();
                case DiscountTypeDefinitions.Percent:
                    return new decimal(discount.PercentOff / 100);
                default:
                    throw new NotSupportedException($"Discount type {discount.Type} not supperted");
            }
        }
    }
}
