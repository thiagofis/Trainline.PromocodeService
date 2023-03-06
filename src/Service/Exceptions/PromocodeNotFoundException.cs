using System;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Exceptions
{
    public class PromocodeNotFoundException : Exception
    {
        public string ErrorCode { get; set; }

        public PromocodeNotFoundException()
        {
            ErrorCode = ErrorCodes.GetErrorCode(null);
        }

        public PromocodeNotFoundException(string message) : base(message)
        {
            ErrorCode = ErrorCodes.PromocodeNotFound;
        }
    }
}
