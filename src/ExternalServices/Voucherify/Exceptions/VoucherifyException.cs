using System;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{

    public class VoucherifyException : Exception
    {
        public Error Error { get; }

        public VoucherifyException(Error error) : base(string.Empty)
        {
            Error = error;
        }
    }
}
