using System;

namespace Trainline.PromocodeService.Common.Exceptions
{
    public class InvalidPromocodeException : Exception
    {
        public string ErrorCode { get; set; }
        public string Value { get; set; }

        public InvalidPromocodeException()
        {
            ErrorCode = ErrorCodes.GetErrorCode(null);
            Value = string.Empty;
        }

        public InvalidPromocodeException(string message)
        {
            ErrorCode = ErrorCodes.InvalidPromocode;
            Value = message;
        }
    }
}
