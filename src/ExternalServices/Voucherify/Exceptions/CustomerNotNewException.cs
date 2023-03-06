using System;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class CustomerNotNewException : Exception
    {
        public CustomerNotNewException() { }

        public CustomerNotNewException(string message) : base(message) { }
    }
}
