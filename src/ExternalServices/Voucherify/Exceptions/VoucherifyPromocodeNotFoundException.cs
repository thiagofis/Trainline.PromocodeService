using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class VoucherifyPromocodeNotFoundException : Exception
    {
        public string Value { get; set; }

        public VoucherifyPromocodeNotFoundException()
        {
            Value = string.Empty;
        }

        public VoucherifyPromocodeNotFoundException(string voucherCode)
        {
           Value = voucherCode;
        }
    }
}
