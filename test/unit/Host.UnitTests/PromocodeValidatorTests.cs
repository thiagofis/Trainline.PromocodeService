using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.UnitTests
{
    public class PromocodeValidatorTests
    {
        private const string TestCode = "Code123";
        private const string TestPromocodeId = "TestPromocodeId123";
        private Mock<IDateTimeProvider> _dateTimeProvider;
        private Mock<IPromocodeService> _promocodeService;
        private PromocodeValidator _validator;
        private DateTime _now;
        private Promocode _promocode;

        [SetUp]
        public void Setup()
        {
            _now = new DateTime(2020, 02, 01);
            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _dateTimeProvider.Setup(x => x.UtcNow).Returns(() => _now);
            _promocode = new Promocode
            {
                Id = 1,
                Code = TestCode,
                ValidityStartDate = new DateTime(2020, 01, 01),
                ValidityEndDate = new DateTime(2020, 03, 01),
                RedeemedQuantity = 0,
                RedemptionQuantity = 1,
                CurrencyCode = "GBP",
                PromocodeId = TestPromocodeId
            };
            _promocodeService = new Mock<IPromocodeService>();
            _promocodeService.Setup(x => x.GetByPromocodeId(TestPromocodeId)).ReturnsAsync(() => _promocode);
            _validator = new PromocodeValidator(_dateTimeProvider.Object, _promocodeService.Object);
        }

        [Test]
        public async Task PromocodeNotFound_ThrowsWithPromocodeNotFound()
        {
            _promocode = null;

            var error = await ValidateThrows(TestCode);

            var errorCode = error.ErrorCodes.Single();
            Assert.AreEqual(ErrorCodes.PromocodeNotFound, errorCode);
        }

        [Test]
        public async Task PromocodeNotStarted_ThrowsWithPromocodeHaveNotStarted()
        {
            _promocode.ValidityStartDate = new DateTime(2020, 03, 01);
            
            var error = await ValidateThrows(TestPromocodeId);

            var errorCode = error.ErrorCodes.Single();
            Assert.AreEqual(ErrorCodes.PromocodeHaveNotStarted, errorCode);
        }

        [Test]
        public async Task PromocodeExpired_ThrowsWithPromocodeExpired()
        {
            _promocode.ValidityEndDate = new DateTime(2020, 01, 01);

            var error = await ValidateThrows(TestPromocodeId);

            var errorCode = error.ErrorCodes.Single();
            Assert.AreEqual(ErrorCodes.PromocodeExpired, errorCode);
        }

        [Test]
        public async Task PromocodeRedeemed_ThrowsWithPromocodeAlreadyRedeemed()
        {
            _promocode.RedeemedQuantity = 2;
            _promocode.RedemptionQuantity = 1;

            var error = await ValidateThrows(TestPromocodeId);

            var errorCode = error.ErrorCodes.Single();
            Assert.AreEqual(ErrorCodes.PromocodeAlreadyRedeemed, errorCode);
        }


        [Test]
        public async Task PromocodeWrongCurrencyCode_ThrowsWithPromocodeCurrencyNotApplicable()
        {
            _promocode.CurrencyCode = "WRONG";

            var error = await ValidateThrows(TestPromocodeId);

            var errorCode = error.ErrorCodes.Single();
            Assert.AreEqual(ErrorCodes.PromocodeCurrencyNotApplicable, errorCode);
        }


        [Test]
        public async Task PromocodeMultipleErrors_ThrowsWithAllErrorCodes()
        {
            _promocode.ValidityStartDate = new DateTime(2020, 03, 01);
            _promocode.ValidityEndDate = new DateTime(2020, 01, 01);
            _promocode.RedeemedQuantity = 2;
            _promocode.RedemptionQuantity = 1;
            _promocode.CurrencyCode = "WRONG";

            _promocodeService.Setup(x => x.GetByPromocodeId(TestPromocodeId)).ReturnsAsync(_promocode);

            var error = await ValidateThrows(TestPromocodeId);

            var errorCodes = error.ErrorCodes.ToList();
            Assert.Contains(ErrorCodes.PromocodeHaveNotStarted, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeExpired, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeAlreadyRedeemed, errorCodes);
            Assert.Contains(ErrorCodes.PromocodeCurrencyNotApplicable, errorCodes);
        }


        private async Task<PromocodeValidatorException> ValidateThrows(string promocodeId)
        {
            PromocodeValidatorException error = null;
            try
            {
                await _validator.Validate(promocodeId);

            }
            catch (PromocodeValidatorException e)
            {
                error = e;
            }

            return error;
        }
    }
}
