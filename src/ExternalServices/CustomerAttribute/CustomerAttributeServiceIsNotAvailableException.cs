using System;

namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute
{
    public class CustomerAttributeServiceIsNotAvailableException : Exception
    {
        public CustomerAttributeServiceIsNotAvailableException(string message) : base(message)
        {
        }

    }
}
