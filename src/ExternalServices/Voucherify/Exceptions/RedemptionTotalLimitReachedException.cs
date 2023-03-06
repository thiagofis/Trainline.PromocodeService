using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class RedemptionTotalLimitReachedException : Exception
    {
        public RedemptionTotalLimitReachedException() { }

        public RedemptionTotalLimitReachedException(string message) : base(message) {}

    }
}
