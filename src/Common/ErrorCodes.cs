using System.Collections.Generic;

namespace Trainline.PromocodeService.Common
{
    public static class ErrorCodes
    {
        public const string AllUnknownError = "41708.1";
        public const string InvalidState = "41708.2";
        public const string PromocodeUnknownValidationIssue = "41708.8";
        public const string PromocodeNotFound = "41708.9";
        public const string PromocodeHaveNotStarted = "41708.10";
        public const string PromocodeExpired = "41708.11";
        public const string PromocodeVendorNotMatching = "41708.12";
        public const string PromocodeExcludedVendorMatched = "41708.13";
        public const string PromocodeCurrencyNotApplicable = "41708.14";
        public const string PromocodeOrderThenMinimumSpend = "41708.15";
        public const string PromocodeAlreadyRedeemed = "41708.16";
        public const string RedemptionAlreadyReinstated = "41708.17";
        public const string ProductUriNotValid = "41708.18";
        public const string InvalidPromocode = "41708.19";
        public const string CustomerIsNotEligibleForTheCampaign = "41708.20";
        public const string RedemptionTotalLimitReached = "41708.21";
        public const string CustomerImplicitRegistrationFailed = "41708.22";
        public const string ContextNotFound = "41708.23";
        public const string ThirdPartyServiceUnavailableOrNotWorkingAsExpected = "41708.24";
        public const string PromocodeCustomerNotNew = "41708.25";
        public const string ExcludedProductTypeMatched = "41708.26";
        
        private static readonly Dictionary<string, string> ErrorPrefixToError = new Dictionary<string, string>
        {
            { ErrorMessagePrefix.OrderMinimumSpendPrefix, PromocodeOrderThenMinimumSpend },
            { ErrorMessagePrefix.VendorNotMatchingPrefix, PromocodeVendorNotMatching },
            { ErrorMessagePrefix.ExcludedVendorMatchedPrefix, PromocodeExcludedVendorMatched },
            { ErrorMessagePrefix.CurrencyCodeNotMatchingPrefix, PromocodeCurrencyNotApplicable },
            { ErrorMessagePrefix.ExcludedTotalNumberRedemptionsPrefix, RedemptionTotalLimitReached },
            { ErrorMessagePrefix.VoucherifyCustomerNotNewPrefix,  PromocodeCustomerNotNew },
            { ErrorMessagePrefix.ExcludedProductTypeMatchedPrefix, ExcludedProductTypeMatched  }
        };

        public static string GetErrorCode(string errorPrefix)
        {
            return string.IsNullOrWhiteSpace(errorPrefix)
                ? PromocodeUnknownValidationIssue
                : (ErrorPrefixToError.ContainsKey(errorPrefix) ? ErrorPrefixToError[errorPrefix]
                                                                : PromocodeUnknownValidationIssue);
        }
    }
}
