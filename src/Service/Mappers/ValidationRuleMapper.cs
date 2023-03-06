using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class ValidationRuleMapper : IValidationRuleMapper
    {
        public const string PromocodeNotStarted = "PromocodeNotStarted";
        public const string PromocodeExpired = "PromocodeExpired";

        public IList<Repository.Entities.ValidationRule> Map(ValidationContainer validationContainer)
        {
            var list = new List<Repository.Entities.ValidationRule>();

            MapValidationContainer(validationContainer.Rules.RuleDetails, validationContainer.Id, list);

            return list;
        }

        public IEnumerable<ValidationRule> Map(Repository.Entities.Promocode promocode, IEnumerable<Repository.Entities.ValidationRule> validationRules)
        {
            var promocodeBasedValidationRules = MapPromocodeValidationRules(promocode);
            return promocodeBasedValidationRules.Concat(validationRules.Select(x => new ValidationRule(x.Name, x.Value)));
        }

        private IEnumerable<ValidationRule> MapPromocodeValidationRules(Repository.Entities.Promocode promocode)
        {
            yield return new ValidationRule(PromocodeNotStarted, DateTime.SpecifyKind(promocode.ValidityStartDate, DateTimeKind.Utc).ToString("O"));
            yield return new ValidationRule(PromocodeExpired, DateTime.SpecifyKind(promocode.ValidityEndDate, DateTimeKind.Utc).ToString("O"));
        }

        private static void MapValidationContainer(IEnumerable<ValidationRuleDetails> rules, string id, ICollection<Repository.Entities.ValidationRule> list)
        {
            foreach (var ruleDetails in rules)
            {
                if (ruleDetails.Rules?.RuleDetails != null && ruleDetails.Rules.RuleDetails.Any())
                {
                    MapValidationContainer(ruleDetails.Rules.RuleDetails, id, list);
                }

                if (ruleDetails.Error?.Message != null && ErrorMessagePrefix.All.Contains(ErrorMessagePrefix.GetPrefix(ruleDetails.Error.Message)))
                {
                    list.Add(new Repository.Entities.ValidationRule
                    {
                        RuleId = id,
                        Name = ErrorMessagePrefix.GetPrefix(ruleDetails.Error.Message),
                        Value = ErrorMessagePrefix.GetValue(ruleDetails.Error.Message)
                    });
                }
            }
        }
    }
}
