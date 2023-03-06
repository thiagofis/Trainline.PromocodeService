using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class ValidationRulesAssignments
    {
        public string Object { get; set; }

        public int Total { get; set; }

        public List<ValidationRulesAssignment> Data { get; set; }
    }
}
