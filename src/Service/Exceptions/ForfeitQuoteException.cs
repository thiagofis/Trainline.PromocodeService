using System;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Exceptions
{

    public class ForfeitQuoteException : Exception
    {
        public string PromocodeId { get; set; }

        public string RedemptionId { get; set; }

        public string QuoteId { get; set; }

        public string VoidableId { get; set; }

        public ForfeitQuoteException(string promocodeId, string redemptionId, string quoteId, string voidableId)
            : base("Cannot forfeit quote as another quote for the same voidable has already been forfeit.")
        {
            PromocodeId = promocodeId;
            RedemptionId = redemptionId;
            QuoteId = quoteId;
            VoidableId = voidableId;
        }
    }
}
