using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Mappers;
using Promocode = Trainline.PromocodeService.Contract.Promocode;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{
    [TestFixture]
    public class CreatePromocodeFeature : AcceptanceTestBase
    {
        [Test]
        public async Task CreatePromocode()
        {
            var voucher = Given.AVoucherWithCode("testvoucher", null, 1, 1);

            var response = await When.IPost<CreatePromocode, Promocode>(new CreatePromocode
            {
                Code = "testvoucher"
            }, "/promocodes");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.Created);

            Assert.AreEqual("testvoucher", response.Content.Code);
            Assert.AreEqual(voucher.StartDate.ToString("O"), response.Content.ValidityStartDate);
            Assert.AreEqual(voucher.ExpirationDate.ToString("O"), response.Content.ValidityEndDate);
            Assert.AreEqual(voucher.Redemption.Quantity, response.Content.Redemption.RedemptionQuantity);
            Assert.AreEqual(voucher.Redemption.RedeemedQuantity, response.Content.Redemption.RedeemedQuantity);
        }

        [Test]
        public async Task CreatePromocode_ReturnsValidationRules()
        {
            var ruleId = "ruleId123";
            Given.AValidationRule(ruleId);
            var voucher = Given.AVoucherWithCode("testvoucher", ruleId);
            var response = await When.IPost<CreatePromocode, Promocode>(new CreatePromocode
            {
                Code = "testvoucher"
            }, "/promocodes");


            Then.TheResponseCodeShouldBe(response, HttpStatusCode.Created);

            var validationRules = response.Content.ValidationRules.ToList();
            Assert.AreEqual(5, validationRules.Count());

            var validationRule = validationRules.Single(x => x.Name == "OrderMinimumSpend");
            Assert.AreEqual("100", validationRule.Value);

            validationRule = validationRules.Single(x => x.Name == "VendorNotMatching");
            Assert.AreEqual("ATOC", validationRule.Value);

            validationRule = validationRules.Single(x => x.Name == "CurrencyCodeNotMatching");
            Assert.AreEqual("GBP", validationRule.Value);

            validationRule = validationRules.Single(x => x.Name == ValidationRuleMapper.PromocodeNotStarted);
            Assert.AreEqual(voucher.StartDate.ToString("O"), validationRule.Value);

            validationRule = validationRules.Single(x => x.Name == ValidationRuleMapper.PromocodeExpired);
            Assert.AreEqual(voucher.ExpirationDate.ToString("O"), validationRule.Value);
        }

        [Test]
        public async Task CreatePromocode_WithMaxedOutReedemedVouchersInCampaign_Throws_RedemptionTotalLimitReached()
        {
            var voucher = Given.AVoucherWithCode("testvoucher", null, 1, 1);

            var campaign = Given.AnInvalidValidCampaign(voucher.CampaignId);

            var response = await When.IPost<CreatePromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(new CreatePromocode
            {
                Code = "testvoucher"
            }, "/promocodes");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.RedemptionTotalLimitReached, response.Content.Errors.FirstOrDefault()?.Code);

        }

        [Test]
        public async Task CreatePromocode_WithAnInactiveVoucher_Throws_InvalidPromocode()
        {
            var voucher = Given.AnInactiveVoucherWithCode("testvoucher", null, 1, 1);

            var response = await When.IPost<CreatePromocode, Trainline.NetStandard.Exceptions.Contracts.ErrorResponse>(new CreatePromocode
            {
                Code = "testvoucher"
            }, "/promocodes");

            Then.TheResponseCodeShouldBe(response, HttpStatusCode.BadRequest);

            Assert.AreEqual(1, response.Content.Errors.Count);
            Assert.AreEqual(ErrorCodes.InvalidPromocode, response.Content.Errors.FirstOrDefault()?.Code);
        }
    }
}
