using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class PromocodeExpiredException : Exception
    {
        public PromocodeExpiredException() { }

        public PromocodeExpiredException(string message) : base(message) { }
    }
}
