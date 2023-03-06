using System.Collections.Generic;

namespace Trainline.PromocodeService.Common
{
    public static class AllowedCurrencies
    {
        public static IList<string> All = typeof(AllowedCurrencies).GetAllStringFields();
        public const string EUR = "EUR";
        public const string GBP = "GBP";

    }
}
