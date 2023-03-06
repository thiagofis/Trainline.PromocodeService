using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Extensions
{
    public static class VoucherifyPriceExtensions
    {
        public static int ToVoucherifyPrice(this decimal price) => Decimal.ToInt32(price * 100);
        public static decimal ToPromoPrice(this int price) => Convert.ToDecimal(price) / 100;
    }
}
