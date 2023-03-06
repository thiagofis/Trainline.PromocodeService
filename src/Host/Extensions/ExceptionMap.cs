using System.Net;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using System;
using System.Linq;
using Trainline.NetStandard.Exceptions.Contracts;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.CardType;

namespace Trainline.PromocodeService.Host.Extensions
{

    public static class ExceptionMap
    {
        public static Error MapError(VoucherifyException exception)
        {
            return new Error(Severity.Correctable, exception.Error.HttpStatusCode.ToString(), $"{exception.Error.Message} - {exception.Error.Details}");

            //if we want to new up th object Meta cannot be null 
            //return new NetStandard.Exceptions.Contracts.Error
            //{
            //    Code = voucherifyException.Error.Code.ToString(),
            //    Detail = $"{voucherifyException.Error.Message} - {voucherifyException.Error.Details}",
            //    Meta = new Metadata()
            //};
        }

        public static HttpStatusCode MapStatusCode(VoucherifyException exception)
        {
            return (HttpStatusCode)exception.Error.HttpStatusCode;
        }

        public static Error MapError(DiscountCardClientException exception)
        {
            return new Error(Severity.Correctable, exception.Error.HttpStatusCode.ToString(), $"{exception.Error.Message} - {exception.Error.Details}");
        }

        public static HttpStatusCode MapStatusCode(DiscountCardClientException exception)
        {
            return (HttpStatusCode)exception.Error.HttpStatusCode;
        }

        public static Error MapError(CardTypeClientException exception)
        {
            return new Error(Severity.Correctable, exception.Error.HttpStatusCode.ToString(), $"{exception.Error.Message} - {exception.Error.Details}");
        }

        public static HttpStatusCode MapStatusCode(CardTypeClientException exception)
        {
            return (HttpStatusCode)exception.Error.HttpStatusCode;
        }
        public static ErrorResponse MapErrorResult(PromocodeValidatorException promocodeValidatorException, string serviceName)
        {

            var errors = promocodeValidatorException
                .ErrorCodes
                .Select(errorCode => new Error(Severity.Correctable, errorCode, "", serviceName))
                .ToList();

            return new ErrorResponse
            {
                Errors = errors
            };
        }
    }
}
