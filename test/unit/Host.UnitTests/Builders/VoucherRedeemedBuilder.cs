using System;
using System.Collections.Generic;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.UnitTests.Builders
{
    public class VoucherRedeemedBuilder
    {
        private readonly Redeemed _redeemed;

        public VoucherRedeemedBuilder(Redeemed redeemed)
        {
            this._redeemed = redeemed;
        }

        public static VoucherRedeemedBuilder ForAmountDiscount(decimal discount)
        {
            return new VoucherRedeemedBuilder(new Redeemed()
            {
                Voucher = new RedeemedVoucher
                {
                    Discount = new Discount()
                    {
                        AmountOff = discount.ToVoucherifyPrice(),
                        Type = DiscountTypeDefinitions.Amount
                    },
                    ApplicableTo = new ApplicableTo
                    {
                        Object = "list",
                        Total = 1,
                        Data = new List<ApplicableInfo>()
                    },
                    Redemption = new VoucherRedemptions
                    {
                        RedeemedQuantity = 10,
                        Quantity = 10
                    }
                },
                Result = "SUCCESS"
            });
        }

        public VoucherRedeemedBuilder ForProduct(string productId)
        {
            this._redeemed.ApplicableTo.Data.Add(new ApplicableInfo
            {
                Object = "product",
                Id = Guid.NewGuid().ToString(),
                SourceId = productId
            });

            return this;
        }

        public VoucherRedeemedBuilder ForPromocode(string code, int redeemedQuantity, int quantity, string currencyCode, string productType)
        {
            _redeemed.Voucher.Code = code;
            _redeemed.Voucher.Redemption = new VoucherRedemptions
            {
                RedeemedQuantity = redeemedQuantity,
                Quantity = quantity
            };
            _redeemed.Voucher.Metadata = new Dictionary<string,string>
            {
                { MetadataKeys.CurrencyCode, currencyCode },
                { MetadataKeys.ProductType, productType}
            };
            return this;
        }

        public static implicit operator Redeemed(VoucherRedeemedBuilder builder)
        {
            return builder._redeemed;
        }
    }
}
