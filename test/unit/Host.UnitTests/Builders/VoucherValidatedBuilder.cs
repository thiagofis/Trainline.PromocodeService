using System;
using System.Collections.Generic;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.UnitTests.Builders
{
    public class VoucherValidatedBuilder
    {
        private readonly Validated _validated;

        public VoucherValidatedBuilder(Validated validated)
        {
            this._validated = validated;
        }

        public static VoucherValidatedBuilder ForAmountDiscount(decimal discount)
        {
            return new VoucherValidatedBuilder(new Validated()
            {
                Valid = true,
                Discount = new Discount
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
                Metadata = new Dictionary<string, string>()
            });
        }

        public static VoucherValidatedBuilder ForPercentDiscount(double discount)
        {
            return new VoucherValidatedBuilder(new Validated()
            {
                Valid = true,
                Discount = new Discount
                {
                    PercentOff = discount,
                    Type = DiscountTypeDefinitions.Percent
                },
                ApplicableTo = new ApplicableTo
                {
                    Object = "list",
                    Total = 1,
                    Data = new List<ApplicableInfo>()
                },
                Metadata = new Dictionary<string, string>()
            });
        }

        public VoucherValidatedBuilder ForProduct(string productId)
        {
            this._validated.ApplicableTo.Data.Add(new ApplicableInfo
            {
                Object = "product",
                Id = Guid.NewGuid().ToString(),
                SourceId = productId
            });

            return this;
        }

        public static implicit operator Validated(VoucherValidatedBuilder builder)
        {
            return builder._validated;
        }
    }
}
