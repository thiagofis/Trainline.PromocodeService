using System;

namespace Trainline.PromocodeService.Model
{
    public class PromoQuote
    {
        public long Id { get; set; }

        public string PromoQuoteId { get; set; }

        public string ReferenceId { get; set; }

        public Uri ProductUri { get; set; }

        public Money DeductionAmount { get; set; }

        public QuoteStatus Status { get; set; }

        public string Hash { get; set; }
    }
}
