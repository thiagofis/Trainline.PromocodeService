using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public interface IVoucherifyResponse
    {
        Discount Discount { get; }
        
        ApplicableTo ApplicableTo { get; }

        Dictionary<string, string> Metadata { get; }
    }
}
