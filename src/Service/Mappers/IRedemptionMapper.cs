using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Mappers
{
    public interface IRedemptionMapper
    {
        Redemption Map(Redeemed response);

        Model.Redemption Map(Redemption redemption);
    }
}
