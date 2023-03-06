using System.Collections.Generic;

namespace Trainline.PromocodeService.Contract
{
    public class PromoLink
    {
        public string Id { get; set; }
        public string RedemptionId { get; set; }

        public Dictionary<string, Link> Links { get; set; }
    }
}
