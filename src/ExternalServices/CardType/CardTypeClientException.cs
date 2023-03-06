using System;
using Trainline.PromocodeService.ExternalServices.CardType.Contract;

namespace Trainline.PromocodeService.ExternalServices.CardType
{
    public class CardTypeClientException : Exception
    {
        public Error Error { get; }

        public CardTypeClientException(Error error) : base(string.Empty)
        {
            Error = error;
        }
    }
}
