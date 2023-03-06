using System.Collections.Generic;

namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class Ledger
    {
        public long Id { get; set; }

        public string PromocodeId { get; set; }

        public string RedemptionId { get; set; }

        public string CurrencyCode { get; set; }

        public decimal PromoAmount { get; set; }

        public List<LedgerLine> Lines { get; set; }

        public List<ProductInfo> Products { get; set; }

        public List<PromoQuote> Quotes { get; set; }
    }
}
