using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Redeemed : IVoucherifyResponse
    {
        public Discount Discount => this.Voucher.Discount;

        public ApplicableTo ApplicableTo => this.Voucher.ApplicableTo;

        public Dictionary<string, string> Metadata => this.Voucher.Metadata;

        public string Id { get; set; }

        public string Result { get; set; }

        public RedeemedVoucher Voucher { get; set; }
    }
}
