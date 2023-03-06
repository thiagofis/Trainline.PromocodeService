using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Order
    {
        public int Amount { get; set; }

        public string SourceId { get; set; }

        public List<Item> Items { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}
