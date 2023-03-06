using System.Collections.Generic;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Mappers
{
    public interface IValidationRuleMapper
    {
        IList<Repository.Entities.ValidationRule> Map(ValidationContainer validationContainer);
        IEnumerable<ValidationRule> Map(Repository.Entities.Promocode promocode, IEnumerable<Repository.Entities.ValidationRule> validationRules);
    }
}
