using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Contract
{
    public class PromoQuote
    {
        public string Id { get; set; }

        public string ReferenceId { get; set; }

        public Uri ProductUri { get; set; }

        public Money DeductionAmount { get; set; }

        public QuoteStatus Status { get; set; }

        public Dictionary<string, Link> Links { get; set; }
    }
}
