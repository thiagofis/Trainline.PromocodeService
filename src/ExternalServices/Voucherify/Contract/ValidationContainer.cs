using System.Collections.Generic;
using Newtonsoft.Json;
using Trainline.PromocodeService.ExternalServices.Serializers;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    [JsonConverter(typeof(ValidationSerializer))]
    public class ValidationContainer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ValidationRulesContainer Rules { get; set; }
    }

    public class ValidationRulesContainer
    {
        public IList<ValidationRuleDetails> RuleDetails { get; set; }

        public string Logic { get; set; }
    }

    public class ValidationRuleDetails
    {
        public string Name { get; set; }

        public ValidationRulesContainer Rules { get; set; }

        public ValidationError Error { get; set; }
    }

    public class ValidationError
    {
        public string Message { get; set; }
    }
}
