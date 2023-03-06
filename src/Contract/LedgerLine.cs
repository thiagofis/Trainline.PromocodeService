using System;

namespace Trainline.PromocodeService.Contract
{
    public class LedgerLine
    {
        public Uri ProductUri { get; set; }

        public decimal Amount { get; set; }

        public string LinkId { get; set; }
    }
}
