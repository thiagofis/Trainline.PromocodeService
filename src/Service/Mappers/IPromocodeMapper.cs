using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;
using Promocode = Trainline.PromocodeService.Model.Promocode;
using Discount = Trainline.PromocodeService.Model.Discount;

namespace Trainline.PromocodeService.Service.Mappers
{

    public interface IPromocodeMapper
    {
        Repository.Entities.Promocode Map(Voucher voucher);

        Promocode Map(Repository.Entities.Promocode promocodeEntity, IEnumerable<Repository.Entities.ValidationRule> validationRuleEntities);

        Discount Map(ExternalServices.Voucherify.Contract.Discount discount);
    }
}
