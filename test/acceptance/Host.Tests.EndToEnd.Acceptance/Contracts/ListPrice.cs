using System.Collections.Generic;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts
{
    public class ListPrice
    {
        public decimal Amount { get; set; }

        public List<ListPriceBreakdownItem> Breakdown { get; set; } = new List<ListPriceBreakdownItem>();

        public string CurrencyCode { get; set; }
    }
}
