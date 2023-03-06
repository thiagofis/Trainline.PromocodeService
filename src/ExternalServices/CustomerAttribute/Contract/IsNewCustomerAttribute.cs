using System;

namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract
{
    public class IsNewCustomerAttribute
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProvenanceSource { get; set; }
        public string ProvenanceId { get; set; }
    }
}
