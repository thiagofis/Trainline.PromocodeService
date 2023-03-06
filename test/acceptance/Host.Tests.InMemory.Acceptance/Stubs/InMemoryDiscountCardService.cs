using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.DiscountCard;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryDiscountCardService : IDiscountCardClient
    {
        public const string P1Y = "P1Y";
        public const string P3Y = "P3Y";

        private Dictionary<Uri, DiscountCard> DiscountCard = new Dictionary<Uri, DiscountCard>();

        public Task<DiscountCard> GetDiscountCardDetailsAsync(Uri productUri)
        {
            if (DiscountCard.ContainsKey(productUri))
            {
                return Task.FromResult(DiscountCard[productUri]);
            }
            return null;
        }

        public void SetDiscountCardDetails(Uri productUri, DiscountCard discountCard)
        {
            DiscountCard[productUri] = discountCard;
        }

    }
}
