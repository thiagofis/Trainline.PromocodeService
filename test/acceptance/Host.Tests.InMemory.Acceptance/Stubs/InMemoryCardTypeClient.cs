using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.CardType;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryCardTypeClient : ICardTypeClient
    {
        private Dictionary<Uri, string> CodeForDiscountCard = new Dictionary<Uri, string>();
        public const string Code = "urn:trainline:atoc:card:NEW";
        public const string AvantageAdultCode = "urn:trainline:sncf:card:AvantageAdulte";

        public Task<string> GetCardTypeCodeAsync(Uri discountCardUri)
        {
            if (CodeForDiscountCard.ContainsKey(discountCardUri))
            {
                return Task.FromResult(CodeForDiscountCard[discountCardUri]);
            }
            return null;
        }

        public void SetCardTypeCodeAsync(Uri discountCardUri, string discountCardCode)
        {
            CodeForDiscountCard[discountCardUri] = discountCardCode;
        }
    }
}
