using System;
using NUnit.Framework;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Controllers;
using Trainline.PromocodeService.Service.Exceptions;

namespace Trainline.PromocodeService.Host.UnitTests.Controllers
{
    [TestFixture]
    public class InvoicesExtensionsTests
    {
        [TestCase(AllowedCurrencies.EUR)]
        [TestCase(AllowedCurrencies.GBP)]
        public void GivenAInvoiceWithASupportedCurrency_WhenValidateIt_ThenShouldNotThrowAnyException(string currencyCode)
        {
            var invoices = new Invoice[]
            {
                new Invoice { CurrencyCode = currencyCode }
            };

            Action action = () => invoices.ValidateCurrency();

            Assert.That(action, Throws.Nothing);
        }

        [Test]
        public void GivenAInvoiceWithAUnsupportedCurrency_WhenValidateIt_ThenAPromocodeCurrencyNotApplicableExceptionIsThrown()
        {
            var invoices = new Invoice[]
            {
                new Invoice { CurrencyCode = "BRL" }
            };

            TestDelegate action = () => invoices.ValidateCurrency();

            var ex = Assert.Throws<PromocodeCurrencyNotApplicableException>(action);
            Assert.AreEqual($"BRL is not a supported currency. Only EUR and GBP are supported.", ex.Value);
        }
    }
}
