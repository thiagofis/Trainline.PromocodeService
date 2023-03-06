using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class RedemptionMapper : IRedemptionMapper
    {
        public Redemption Map(Redeemed response)
        {
            return new Redemption
            {
                Code = response.Voucher.Code,
                RedemptionId = response.Id,
            };
        }

        public Model.Redemption Map(Redemption redemption)
        {
            return new Model.Redemption
            {
                PromocodeId = redemption.PromocodeId,
                Id = redemption.RedemptionId
            };
        }
    }
}
