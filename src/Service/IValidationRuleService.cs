using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service
{
    public interface IValidationRuleService
    {
        Task<IEnumerable<ValidationRule>> Get(string validationRuleId);
    }
}
