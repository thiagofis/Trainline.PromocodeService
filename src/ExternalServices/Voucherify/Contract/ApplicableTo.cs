using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class ApplicableTo
    {
        public string Object { get; set; }

        public int Total { get; set; }

        public List<ApplicableInfo> Data { get; set; }
    }
}
