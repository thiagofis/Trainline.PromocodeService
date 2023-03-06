using System;
using System.Linq;
using Trainline.PromocodeService.Contract;

namespace Trainline.PromocodeService.Acceptance.Helpers
{
    public static class RedeemedExtensions
    {
        public static Uri QuoteUri(this Redeemed redeemed) => redeemed.Links.Single(x => x.Key == "quotes").Value.Href;

        public static Uri LedgerUri(this Redeemed redeemed) => redeemed.Links.Single(x => x.Key == "ledger").Value.Href;
    }
}
