using System;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.ExternalServices.CardType
{
    public interface ICardTypeClient
    {
        Task<string> GetCardTypeCodeAsync(Uri cardTypeUri);
    }
}
