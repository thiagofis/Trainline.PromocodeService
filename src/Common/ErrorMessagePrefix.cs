using System.Collections.Generic;

namespace Trainline.PromocodeService.Common
{
    public static class ErrorMessagePrefix
    {
        public const string OrderMinimumSpendPrefix = "OrderMinimumSpend";
        public const string VendorNotMatchingPrefix = "VendorNotMatching";
        public const string ExcludedVendorMatchedPrefix = "ExcludedVendorMatched";
        public const string CurrencyCodeNotMatchingPrefix = "CurrencyCodeNotMatching";
        public const string ExcludedTotalNumberRedemptionsPrefix = "ExcludedTotalNumberRedemptions";
        public const string VoucherifyCustomerNotNewPrefix = "VoucherifyCustomerNotNew";
        public const string ExcludedProductTypeMatchedPrefix = "ExcludedProductTypeMatched";

        public static IEnumerable<string> All = new List<string>
        {
            OrderMinimumSpendPrefix,
            VendorNotMatchingPrefix,
            ExcludedVendorMatchedPrefix,
            CurrencyCodeNotMatchingPrefix,
            ExcludedTotalNumberRedemptionsPrefix,
            VoucherifyCustomerNotNewPrefix,
            ExcludedProductTypeMatchedPrefix
        };

        public static string GetPrefix(string errorMessage)
        {
            foreach (var errorPrefix in All)
            {
                if (errorMessage.StartsWith(errorPrefix))
                {
                    return errorPrefix;
                }
            }

            return string.Empty;
        }

        public static string GetValue(string errorMessage)
        {
            foreach (var errorPrefix in All)
            {
                if (errorMessage.StartsWith(errorPrefix))
                {
                    return errorMessage.Replace($"{errorPrefix}_", "");
                }
            }

            return string.Empty;
        }
    }
}
