using System;
using System.Collections.Generic;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.UnitTests.Builders
{
    public class VoucherRollbackBuilder
    {
        private readonly RedemptionRollback _rollback;

        public VoucherRollbackBuilder(RedemptionRollback rollback)
        {
            this._rollback = rollback;
        }

        public static VoucherRollbackBuilder ForAmountDiscount(decimal discount)
        {
            return new VoucherRollbackBuilder(new RedemptionRollback()
            {
                Voucher = new Voucher
                {
                    Discount = new Discount()
                    {
                        AmountOff = discount.ToVoucherifyPrice(),
                        Type = DiscountTypeDefinitions.Amount
                    },
                    Redemption = new VoucherRedemptions
                    {
                        RedeemedQuantity = 9,
                        Quantity = 10
                    }
                },
                Result = "SUCCESS"
            });
        }

        public VoucherRollbackBuilder ForPromocode(string code, int redeemedQuantity, int quantity, string currencyCode, string productType)
        {
            _rollback.Voucher.Code = code;
            _rollback.Voucher.Redemption = new VoucherRedemptions
            {
                RedeemedQuantity = redeemedQuantity,
                Quantity = quantity
            };
            _rollback.Voucher.Metadata = new Dictionary<string, string>
            {
                { MetadataKeys.CurrencyCode, currencyCode },
                {MetadataKeys.ProductType, productType}
            };
            return this;
        }

        public VoucherRollbackBuilder ForResult(string result)
        {
            _rollback.Result = result;
            return this;
        }


        public static implicit operator RedemptionRollback(VoucherRollbackBuilder builder)
        {
            return builder._rollback;
        }
    }
}
