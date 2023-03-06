using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Trainline.PromocodeService.Contract;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Features
{
    [TestFixture]
    public class ReinstateRedemptionFeature : AcceptanceTestBase
    {
        private string Code = "ReinstateCode123";
        private string PromocodeId = "PromocodeId123";
        private string RedemptionId = "RedemptionId123";

        [Test]
        public async Task Reinstate()
        {
            var voucher = Given.AVoucherWithCode(Code);
            await Given.APromocode(new Service.Repository.Entities.Promocode()
            {
                Code = Code,
                ValidityStartDate = DateTime.Now.AddDays(-10),
                ValidityEndDate = DateTime.Now.AddDays(10),
                CurrencyCode = "GBP",
                DiscountAmount = 10,
                DiscountType = "AMOUNT",
                Id = 1,
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                PromocodeId = PromocodeId
            });
            Given.ARollback(Code, RedemptionId);

            var response = await When.IPost<dynamic>( $"/promocodes/{PromocodeId}/redemptions/{RedemptionId}/reinstate");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
