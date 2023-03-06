using System;
using System.Collections.Generic;
using System.Text;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public static class VoucherifyValidator
    {
        private const string SuccessResult = "SUCCESS";
        private const string QuantityExceeded = "quantity exceeded";
        private const string RedemptionTotalLimitReached = "redemption does not match validation rules";
        private const string NotNewCustomer = "customer does not match validation rules";
        private const string PromocodeExpired = "voucher expired";
        private const string InactivePromocode = "voucher is disabled";

        public static void EnsureSuccess(this Validated validated)
        {
            if (!validated.Valid)
            {
                if (validated.Reason == NotNewCustomer)
                {
                    throw new CustomerNotNewException("Customer is not new.");
                }
                if (validated.Reason == InactivePromocode)
                {
                    throw new InvalidPromocodeException("Promocode is inactive.");
                }
                if (validated.Reason == QuantityExceeded)
                {
                    throw new QuantityExceededException();
                }
                if (validated.Reason == PromocodeExpired)
                {
                    throw new PromocodeExpiredException("Promocode has expired.");
                }

                if (validated.Reason == RedemptionTotalLimitReached)
                {
                    throw new RedemptionTotalLimitReachedException("Campaign has reached maximum number of vouchers redeemed.");
                }

                throw new NotApplicableException(validated.Error?.Message);
            }
        }

        public static void EnsureSuccess(this Redeemed redeemed)
        {
            if (redeemed.Result != SuccessResult)
            {
                throw new NotApplicableException();
            }
        }
    }
}
