using System.Linq;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Service.Exceptions;

namespace Trainline.PromocodeService.Host.Controllers
{
    public static class InvoicesExtensions
    {
        public static void ValidateCurrency(this Invoice[] invoices)
        {
            var invoiceCurrency = invoices.Select(x => x.CurrencyCode).Distinct().ToList();
            var currency = invoiceCurrency.SingleOrDefault();

            if (!Common.AllowedCurrencies.All.Contains(currency))
            {
                throw new PromocodeCurrencyNotApplicableException(
                    $"{currency} is not a supported currency. Only EUR and GBP are supported.");
            }
        }
    }
}
