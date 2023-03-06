using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Product
    {
        public bool Override { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}
