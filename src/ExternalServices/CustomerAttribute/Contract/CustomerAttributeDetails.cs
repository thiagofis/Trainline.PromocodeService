using System;
using System.Linq;

namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract
{
    public class CustomerAttributeDetails
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public object Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProvenanceSource { get; set; }
        public string ProvenanceId { get; set; }
    }

    public class CustomerAttributes
    {
        public string CustomerId { get; set; }
        public CustomerAttributeDetails[] Attributes{ get; set; }

        public T GetValue<T>(string attributeName)
        {
            var attribute = Attributes?.FirstOrDefault(x => string.Equals(x.Name, attributeName, StringComparison.InvariantCultureIgnoreCase));
            if (attribute?.Value == null)
            {
                return default;
            }

            return (T)attribute.Value;
        }

        public bool IsNewCustomer() => GetValue<bool?>(Common.CustomerAttributeNames.IsNewCustomer) ?? true;

        public static CustomerAttributes New(string customerId)
        {
            return new CustomerAttributes
            {
                CustomerId = customerId,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                    {
                        DataType = Common.CustomerAttributeDataTypes.Boolean,
                        Name = Common.CustomerAttributeNames.IsNewCustomer,
                        Value = true
                    },
                    new CustomerAttributeDetails
                    {
                        DataType = Common.CustomerAttributeDataTypes.Boolean,
                        Name = Common.CustomerAttributeNames.BoughtRailcard,
                        Value = false
                    },
                    new CustomerAttributeDetails
                    {
                        DataType = Common.CustomerAttributeDataTypes.Boolean,
                        Name = Common.CustomerAttributeNames.IsActive,
                        Value = false
                    }
                }
            };
        }
    }
}
