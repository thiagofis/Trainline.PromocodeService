using System;
using System.Collections.Generic;
using System.Text;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Exceptions
{
    public class NotValidProductUriException : Exception
    {
        public string ErrorCode { get; set; }
        public string Value { get; set; }

        public NotValidProductUriException()
        {
            ErrorCode = ErrorCodes.GetErrorCode(null);
            Value = string.Empty;
        }

        public NotValidProductUriException(string message)
        {
            ErrorCode = ErrorCodes.ProductUriNotValid;
            Value = message;
        }
    }
}
