using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Service
{
    public class RedemptionService : IRedemptionService
    {
        private readonly IVoucherifyClient _voucherifyClient;
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly IRedemptionRepository _redemptionRepository;
        private readonly IPromocodeMapper _promocodeMapper;
        private readonly IRedemptionMapper _redemptionMapper;
        private const string SuccessResult = "SUCCESS";

        public RedemptionService(IVoucherifyClient voucherifyClient, IPromocodeRepository promocodeRepository, IRedemptionRepository redemptionRepository, IPromocodeMapper promocodeMapper
            , IRedemptionMapper redemptionMapper)
        {
            _voucherifyClient = voucherifyClient;
            _promocodeRepository = promocodeRepository;
            _redemptionRepository = redemptionRepository;
            _promocodeMapper = promocodeMapper;
            _redemptionMapper = redemptionMapper;
        }

        public async Task<ICollection<Redemption>> GetByPromocodeId(string promocodeId)
        {
            var entities = await _redemptionRepository.GetByPromocodeId(promocodeId);

            return entities?.Select(x => _redemptionMapper.Map(x)).ToList();
        }

        public async Task<Redemption> Get(string promocodeId, string redemptionId)
        {
            var entity = await _redemptionRepository.Get(promocodeId, redemptionId);
            return entity != null ? _redemptionMapper.Map(entity) : null;
        }

        public async Task Reinstate(string promocodeId, string redemptionId)
        {
            var promocode = await _promocodeRepository.GetByPromocodeId(promocodeId);

            if (promocode == null)
            {
                throw new PromocodeNotFoundException();
            }

            var response = await _voucherifyClient.RollbackVoucher(promocode.Code, redemptionId);

            if (response.Result != SuccessResult)
            {
                throw new NotApplicableException();
            }

            var promocodeEntity = _promocodeMapper.Map(response.Voucher);
            promocodeEntity.PromocodeId = promocodeId;
            promocodeEntity.ValidationRuleId = promocode.ValidationRuleId;
            promocodeEntity.RetentionDate = promocode.RetentionDate;

            await _promocodeRepository.Update(promocodeEntity);
        }
    }
}
