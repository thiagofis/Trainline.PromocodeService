using System;
using System.Linq;
using Trainline.NetStandard.StandardHeaders.Services;

namespace Trainline.PromocodeService.Service.Extensions
{
    public static class CustomerExtensions
    {
        public static Uri? GetCustomerUri(this IHeaderService headerService)
        {
            var customerUri = headerService.GetHeader(CustomHeaders.CustomerUri).FirstOrDefault();

            return customerUri != null ? new Uri(customerUri) : null;
        }
    }
}
