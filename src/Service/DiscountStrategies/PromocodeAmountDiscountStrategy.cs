using System;
using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.DiscountStrategies
{
    public class PromocodeAmountDiscountStrategy : IPromocodeDiscountStrategy
    {
        public string DiscountType => DiscountTypeDefinitions.Amount;

        public IEnumerable<DiscountItem> GenerateDiscountItems(Discount discount, ICollection<ProductItem> productItems)
        {
            if (discount?.Type != DiscountTypeDefinitions.Amount)
            {
                throw new NotSupportedException();
            }

            var results = new List<DiscountItem>();

            decimal discountAmount = discount.Amount;
            var sumPrice = productItems.Sum(x => x.Amount);

            var sumOfDiscountAmounts = 0M;
            foreach (var lineItem in productItems)
            {
                var amount = (discountAmount * (lineItem.Amount / sumPrice));
                amount = amount < lineItem.Amount ? amount : lineItem.Amount;
                amount = Math.Round(amount, 2);
                sumOfDiscountAmounts += amount;
                results.Add(new DiscountItem
                {
                    Amount = -amount,
                    ProductId = lineItem.ProductId,
                    ProductUri = lineItem.ProductUri,
                    Vendor = lineItem.Vendor,
                    ProductPrice = lineItem.Amount
                });
            }

            if (results.Any() && discountAmount < sumPrice)
            {
                var adjustment = discountAmount - sumOfDiscountAmounts;
                results[0].Amount -= adjustment;
            }

            return results;
        }
    }
}
