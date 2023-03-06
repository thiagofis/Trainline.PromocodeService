using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Contract
{
    public class Redemption
    {
        public string Id { get; set; }
        public string PromocodeId { get; set; }
        public string CampaignName { get; set; }
        public Dictionary<string, Link> Links { get; set; }
    }
}
