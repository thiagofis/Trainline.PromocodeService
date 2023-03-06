using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class RedemptionRollback
    {
        public string Result { get; set; }

        public Voucher Voucher { get; set; }
    }
}
