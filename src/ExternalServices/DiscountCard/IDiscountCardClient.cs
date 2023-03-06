using System;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.ExternalServices.DiscountCard
{
    public interface IDiscountCardClient
    {
        Task<DiscountCard> GetDiscountCardDetailsAsync(Uri productUri);
    }
}
