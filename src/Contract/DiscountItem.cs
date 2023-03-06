using System;

namespace Trainline.PromocodeService.Contract
{
    public class DiscountItem
    {
        public string ProductId { get; set; }

        public string InvoiceId { get; set; }

        public Uri ProductUri { get; set; }

        public string Vendor { get; set; }

        public string ProductType { get; set; }
        public string ValidityPeriod { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; }


    }
}
