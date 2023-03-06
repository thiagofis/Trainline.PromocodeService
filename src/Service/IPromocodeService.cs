using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service
{
    public interface IPromocodeService
    {
        Task<Promocode> Create(string code);

        Task<Promocode> GetByPromocodeId(string promocodeId);

        Task<Applied> Apply(string code, ICollection<InvoiceInfo> invoices);

        Task<Redeemed> Redeem(string code, ICollection<InvoiceInfo> invoices, DateTime retentionDate);
    }
}
