using System;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Exceptions
{
    public class PromocodeCurrencyNotApplicableException : Exception
    {
        public string ErrorCode { get; set; }
        public string Value { get; set; }

        public PromocodeCurrencyNotApplicableException()
        {
            ErrorCode = ErrorCodes.GetErrorCode(null);
            Value = string.Empty;
        }

        public PromocodeCurrencyNotApplicableException(string message)
        {
            ErrorCode = ErrorCodes.PromocodeCurrencyNotApplicable;
            Value = message;
        }
    }
}
