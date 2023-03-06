using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Service.Caches;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service
{
    public class ValidationRuleService : IValidationRuleService
    {
        private readonly IVoucherifyClient _voucherifyClient;
        private readonly InMemoryCache<IList<ValidationRule>> _cache;
        private readonly IValidationRuleMapper _validationRuleMapper;

        public ValidationRuleService(IValidationRuleMapper validationRuleMapper, IVoucherifyClient voucherifyClient, InMemoryCache<IList<ValidationRule>> cache)
        {
            _validationRuleMapper = validationRuleMapper;
            _voucherifyClient = voucherifyClient;
            _cache = cache;
        }

        public async Task<IEnumerable<ValidationRule>> Get(string validationRuleId)
        {
            if (validationRuleId == null)
            {
                return new List<ValidationRule>();
            }

            return await _cache.GetOrAdd(validationRuleId, () => GetValidationRules(validationRuleId));
        }

        private async Task<IList<ValidationRule>> GetValidationRules(string validationRuleId)
        {
            var validationContainer = await _voucherifyClient.GetValidationRules(validationRuleId);
            return _validationRuleMapper.Map(validationContainer);
        }
    }
}
