using System;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class VoucherifyRedeemException : Exception
    {
        public RedeemedError Error { get; }

        public VoucherifyRedeemException(RedeemedError error) : base(string.Empty)
        {
            Error = error;
        }
    }
}
