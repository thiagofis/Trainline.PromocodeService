using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.DiscountStrategies
{
    public class PromocodeDiscountStrategyFactory : IPromocodeDiscountStrategyFactory
    {
        private readonly Dictionary<string, IPromocodeDiscountStrategy> _promocodeDiscountStrategies;

        public PromocodeDiscountStrategyFactory(IEnumerable<IPromocodeDiscountStrategy> discountStrategies)
        {
            _promocodeDiscountStrategies = discountStrategies.ToDictionary(x => x.DiscountType, v => v);
        }

        public IPromocodeDiscountStrategy Get(Discount discount)
        {
            return _promocodeDiscountStrategies[discount.Type];
        }
    }
}
