using System.Collections.Generic;

namespace Trainline.PromocodeService.Contract
{
    public class Ledger
    {
        public string PromocodeId { get; set; }

        public string RedemptionId { get; set; }

        public string CurrencyCode { get; set; }

        public decimal PromoAmount { get; set; }

        public decimal AvailableAmount { get; set; }

        public IList<LedgerLine> Lines { get; set; }

        public List<ProductInfo> Products { get; set; }

        public List<PromoQuote> Quotes { get; set; }

        public Dictionary<string, Link> Links { get; set; }
    }
}
