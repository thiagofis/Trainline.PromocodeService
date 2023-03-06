using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Serializers
{
    public class ValidationSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            var container = new ValidationContainer();
            foreach (var property in properties)
            {
                switch (property.Name)
                {
                    case "id":
                        container.Id = property.Value.ToString();
                        break;
                    case "name":
                        container.Name = property.Value.ToString();
                        break;
                }
            }

            var validationRuleContainer = GenerateValidationRuleContainer(properties.Where(p => p.Name == "rules").Values().Values(), serializer);

            container.Rules = validationRuleContainer;
            return container;
        }

        private ValidationRulesContainer GenerateValidationRuleContainer(IJEnumerable<JToken> rulesProperties,
            JsonSerializer jsonSerializer)
        {
            var validationRuleContainer = new ValidationRulesContainer
            {
                RuleDetails = new List<ValidationRuleDetails>()
            };

            foreach (var ruleProperty in rulesProperties)
            {
                if (ruleProperty.Type == JTokenType.Property && ((JProperty)ruleProperty).Name == "logic")
                {
                    // no need to deserialize the logic information at the moment
                    continue;
                }
                var validationRuleDetails = new ValidationRuleDetails();

                foreach (var ruleValue in ruleProperty.SelectMany(x => x))
                {
                    var rule = (JProperty) ruleValue;
                    if (rule.Name == "name")
                    {
                        validationRuleDetails.Name = rule.Value.ToString();
                    }

                    if (rule.Name == "rules" && rule.Value.HasValues)
                    {
                        validationRuleDetails.Rules =
                            GenerateValidationRuleContainer(rule.Values(), jsonSerializer);
                    }

                    if (rule.Name == "error" && rule.Value.HasValues)
                    {
                        validationRuleDetails.Error = jsonSerializer.Deserialize<ValidationError>(rule.Value.CreateReader());
                    }
                }
                validationRuleContainer.RuleDetails.Add(validationRuleDetails);

            }

            return validationRuleContainer;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
