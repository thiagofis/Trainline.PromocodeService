using System;

namespace Trainline.PromocodeService.Common.Exceptions
{
    public class NotApplicableException : Exception
    {
        public string ErrorCode { get; set; }
        public string Value { get; set; }

        public NotApplicableException()
        {
            ErrorCode = ErrorCodes.GetErrorCode(null);
            Value = string.Empty;
        }

        public NotApplicableException(string message)
        {
            ErrorCode = ErrorCodes.GetErrorCode(ErrorMessagePrefix.GetPrefix(message));
            Value = ErrorMessagePrefix.GetValue(message);
        }
    }
}
