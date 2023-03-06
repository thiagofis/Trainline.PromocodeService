using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Model
{
    public class ProductItem
    {
        public string ProductId { get; set; }
        public Uri ProductUri { get; set; }
        public string Vendor { get; set; }
        public string ProductType { get; set; }
        public string ValidityPeriod { get; set; }
        public string RailcardCode { get; set; }
        public List<string> CarrierCode { get; set; }
        public List<string> TicketTypeCode { get; set; }
        public decimal Amount { get; set; }
    }
}
