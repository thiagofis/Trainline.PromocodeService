using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service.Mappers
{
    public interface IVoucherifyMapper
    {
        Task<Validation> Map(ICollection<InvoiceInfo> invoices, Uri? customerUri);

        Task<Redeem> MapRedeem(ICollection<InvoiceInfo> invoices, Uri? customerUri);
    }
}
