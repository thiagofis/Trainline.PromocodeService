using System.Collections.Generic;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.DiscountStrategies
{
    public interface IPromocodeDiscountStrategy
    {
        public string DiscountType { get; }
        IEnumerable<DiscountItem> GenerateDiscountItems(Discount discount, ICollection<ProductItem> productItems);
    }
}
