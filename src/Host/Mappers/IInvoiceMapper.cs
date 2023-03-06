using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.Host.Mappers
{
    public interface IInvoiceMapper
    {
        Task<ICollection<Model.InvoiceInfo>> Map(ICollection<Contract.Invoice> invoice);

        ICollection<Contract.DiscountItem> MapDiscounts(IEnumerable<Model.DiscountInvoiceInfo> discountedInvoices);
    }
}
