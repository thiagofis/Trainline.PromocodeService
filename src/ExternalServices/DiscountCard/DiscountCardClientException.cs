using System;
using Trainline.PromocodeService.ExternalServices.DiscountCard.Contract;

namespace Trainline.PromocodeService.ExternalServices.DiscountCard
{
    public class DiscountCardClientException : Exception
    {
        public Error Error { get; }

        public DiscountCardClientException(Error error) : base(string.Empty)
        {
            Error = error;
        }
    }
}
