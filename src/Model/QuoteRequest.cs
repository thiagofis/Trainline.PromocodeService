using System;

namespace Trainline.PromocodeService.Model
{
    public class QuoteRequest
    {
        public Uri ProductUri { get; set; }

        public string ReferenceId { get; set; }

        public Money RefundableAmount { get; set; }

        public Money ListedAmount { get; set; }
    }
}
