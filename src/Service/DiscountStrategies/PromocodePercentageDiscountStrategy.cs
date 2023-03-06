using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.DiscountStrategies
{
    public class PromocodePercentageDiscountStrategy : IPromocodeDiscountStrategy
    {
        public string DiscountType => DiscountTypeDefinitions.Percent;

        public IEnumerable<DiscountItem> GenerateDiscountItems(Discount discount, ICollection<ProductItem> productItems)
        {
            if (discount?.Type != DiscountTypeDefinitions.Percent)
            {
                throw new NotSupportedException();
            }

            if (1 < discount.Amount || discount.Amount < 0)
            {
                throw new InvalidOperationException($"Invalid discount amount of {discount.Amount}.");
            }

            decimal discountAmount = discount.Amount;

            foreach (var lineItem in productItems)
            {
                var amount = lineItem.Amount * discountAmount;
                amount = Math.Round(amount, 2);
                yield return new DiscountItem
                {
                    Amount = -amount,
                    ProductId = lineItem.ProductId,
                    ProductUri = lineItem.ProductUri,
                    Vendor = lineItem.Vendor
                };
            }

        }
    }
}
