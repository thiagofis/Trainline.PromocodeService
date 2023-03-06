using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.DiscountStrategies
{
    public interface IPromocodeDiscountStrategyFactory
    {
        IPromocodeDiscountStrategy Get(Discount discount);
    }
}
