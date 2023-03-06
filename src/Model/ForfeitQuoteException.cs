using System;

namespace Trainline.PromocodeService.Model
{
    public class ForfeitQuoteException : Exception
    {
        public string PromocodeId { get; set; }

        public string RedemptionId { get; set; }

        public string QuoteId { get; set; }

        public string ReferenceId { get; set; }

        public ForfeitQuoteException(string promocodeId, string redemptionId, string quoteId, string referenceId)
            : base("Cannot forfeit quote as another quote for the same reference has already been forfeit.")
        {
            PromocodeId = promocodeId;
            RedemptionId = redemptionId;
            QuoteId = quoteId;
            ReferenceId = referenceId;
        }
    }
}
