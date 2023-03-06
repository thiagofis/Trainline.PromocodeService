using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public interface IVoucherifyClient
    {
        Task<Voucher> GetVoucher(string voucherCode);

        Task<ValidationContainer> GetValidationRules(string validationRuleId);

        Task<Validated> ValidateVoucher(string voucherCode, Validation validation);

        Task<Redeemed> RedeemVoucher(string voucherCode, Redeem redeem);

        Task<RedemptionRollback> RollbackVoucher(string code, string redemptionId);
    }
}
