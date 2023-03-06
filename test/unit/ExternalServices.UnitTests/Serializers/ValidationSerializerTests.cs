using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Trainline.PromocodeService.ExternalServices.Serializers;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace ExternalServices.UnitTests.Serializers
{
    public class ValidationSerializerTests
    {
        [Test]
        public void Id_and_Name_Gets_Deserialized()
        {
            var jsonString = File.ReadAllText("Serializers/Data/validation_no_rule.json");

            var validationContainer = JsonConvert.DeserializeObject<ValidationContainer>(jsonString);
            Assert.AreEqual("testId", validationContainer.Id);
            Assert.AreEqual("Test validation", validationContainer.Name);
        }

        [Test]
        public void Validation_Has_One_Rule_Gets_Deserialized()
        {
            var jsonString = File.ReadAllText("Serializers/Data/validation_one_rule.json");
            var validationContainer = JsonConvert.DeserializeObject<ValidationContainer>(jsonString);
            Assert.AreEqual(1, validationContainer.Rules.RuleDetails.Count);
            Assert.AreEqual("customer.segment" ,validationContainer.Rules.RuleDetails[0].Name);
        }

        [Test]
        public void Validation_Has_Tow_Rules_Gets_Deserialized()
        {
            var jsonString = File.ReadAllText("Serializers/Data/validation_two_rules.json");
            var validationContainer = JsonConvert.DeserializeObject<ValidationContainer>(jsonString);
            Assert.AreEqual(2, validationContainer.Rules.RuleDetails.Count);

            Assert.AreEqual("customer.segment" ,validationContainer.Rules.RuleDetails[0].Name);
            Assert.AreEqual("product.id" ,validationContainer.Rules.RuleDetails[1].Name);
        }

        [Test]
        public void Validation_Has_Tow_Rules_And_One_Rule_Has_Recursive_Rules_Gets_Deserialized()
        {
            var jsonString = File.ReadAllText("Serializers/Data/validation_two_rules_recursive.json");
            var validationContainer = JsonConvert.DeserializeObject<ValidationContainer>(jsonString);
            Assert.AreEqual(2, validationContainer.Rules.RuleDetails.Count);

            Assert.AreEqual("product.metadata" ,validationContainer.Rules.RuleDetails[0].Name);
            Assert.AreEqual(2, validationContainer.Rules.RuleDetails[0].Rules.RuleDetails.Count);
            Assert.AreEqual("VendorNotMatching_ATOC", validationContainer.Rules.RuleDetails[0].Error.Message);

            Assert.AreEqual("product.metadata.aggregated_amount", validationContainer.Rules.RuleDetails[0].Rules.RuleDetails.First().Name);
            Assert.AreEqual("product.metadata.discount_applicable", validationContainer.Rules.RuleDetails[0].Rules.RuleDetails.Last().Name);
            Assert.AreEqual("order.metadata" ,validationContainer.Rules.RuleDetails[1].Name);
        }
    }
}
