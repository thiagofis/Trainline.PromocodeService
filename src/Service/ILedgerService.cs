using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service
{
    public interface ILedgerService
    {
        Task<Ledger> Get(string promocodeId, string redemptionId);

        Task<Ledger> Create(Redeemed redeemed);

        Task<ICollection<PromoQuote>> CreateQuotes(string promocodeId, string redemptionId, IEnumerable<QuoteRequest> quoteRequests);

        Task<PromoQuote> GetQuote(string promocodeId, string redemptionId, string quoteId);

        Task ForfeitQuote(string promocodeId, string redemptionId, string quoteId);

        Task<PromoLink> Link(string promocodeId, string redemptionId, LinkProductRequest linkRequest);

        Task RevertLink(string promocodeId, string redemptionId, string linkId);
    }
}
