using System;
using Trainline.PromocodeService.ExternalServices.DiscountCard.Contract;

namespace Trainline.PromocodeService.ExternalServices.DiscountCard
{
    public class CardDetails
    {
        public string ValidityPeriod { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public DateTime ValidityStartDate { get; set; }
        public Contract.CardType CardType { get; set; }
    }
}
