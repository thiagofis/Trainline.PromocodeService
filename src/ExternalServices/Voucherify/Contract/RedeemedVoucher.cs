using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class RedeemedVoucher : Voucher
    {
        public ApplicableTo ApplicableTo { get; set; }
    }
}
