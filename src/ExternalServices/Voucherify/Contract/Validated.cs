using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Validated : IVoucherifyResponse
    {
        public string Code { get; set; }

        public Discount Discount { get; set; }
        
        public bool Valid { get; set; }

        public string Reason { get; set; }

        public ApplicableTo ApplicableTo { get; set; }
        
        public Error Error { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
