using System;

namespace Trainline.PromocodeService.ExternalServices.Customer.Exceptions
{
    public class CustomerImplicitRegistrationException : Exception
    {
        public CustomerImplicitRegistrationException(string message) : base(message)
        {
        }
    }
}
