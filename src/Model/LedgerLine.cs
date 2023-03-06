using System;

namespace Trainline.PromocodeService.Model
{
    public class LedgerLine
    {
        public long Id { get; set; }

        public Uri ProductUri { get; set; }

        public decimal Amount { get; set; }

        public string LinkId { get; set; }
    }
}
