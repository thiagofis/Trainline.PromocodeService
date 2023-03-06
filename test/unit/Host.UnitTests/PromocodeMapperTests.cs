using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Mappers;
using Promocode = Trainline.PromocodeService.Service.Repository.Entities.Promocode;
using ValidationRule = Trainline.PromocodeService.Service.Repository.Entities.ValidationRule;

namespace Trainline.PromocodeService.Host.UnitTests
{
    public class PromocodeMapperTests
    {
        private Mock<IValidationRuleMapper> _validationRuleMapper;
        private PromocodeMapper _target;

        public PromocodeMapperTests()
        {
            _validationRuleMapper = new Mock<IValidationRuleMapper>();
            _target = new PromocodeMapper(_validationRuleMapper.Object);
        }

        [Test]
        public void Map_WhenCalledWithPromocodeWhichHasNotStarted_ReturnsNotStartedStatus()
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.UtcNow.AddDays(1)
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(PromocodeState.NotStarted, result.State);
        }

        [Test]
        public void Map_WhenCalledWithPromocodeWhichHasExpired_ReturnsExpiredStatus()
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.UtcNow.AddDays(-2),
                ValidityEndDate = DateTime.UtcNow.AddDays(-1)
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(PromocodeState.Expired, result.State);
        }

        [Test]
        public void Map_WhenCalledWithPromocodeWhichHasBeenRedeemed_ReturnsRedeemedStatus()
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.UtcNow.AddDays(-2),
                ValidityEndDate = DateTime.UtcNow.AddDays(5),
                RedeemedQuantity = 1,
                RedemptionQuantity = 1
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(PromocodeState.Redeemed, result.State);
        }

        [Test]
        public void Map_WhenCalledWithPromocodeWhichHasBeenRedeemedAndExpired_ReturnsRedeemedStatus()
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.UtcNow.AddDays(-2),
                ValidityEndDate = DateTime.UtcNow.AddDays(-1),
                RedeemedQuantity = 1,
                RedemptionQuantity = 1
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(PromocodeState.Redeemed, result.State);
        }

        [TestCase(1, 2)]
        [TestCase(1, null)]
        public void Map_WhenCalledWithPromocodeWhichIsOpen_ReturnsOpenStatus(int redeemedQuantity, int? redemptionQuantity)
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.UtcNow.AddDays(-2),
                ValidityEndDate = DateTime.UtcNow.AddDays(5),
                RedeemedQuantity = redeemedQuantity,
                RedemptionQuantity = redemptionQuantity
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(PromocodeState.Open, result.State);
        }

        [Test]
        public void Map_WhenCalled_ReturnsStartAndDateDateInUtcKind()
        {
            var promocodeEntity = new Promocode
            {
                ValidityStartDate = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Unspecified),
                ValidityEndDate = DateTime.SpecifyKind(DateTime.Now.AddDays(1), DateTimeKind.Unspecified)
            };

            IEnumerable<ValidationRule> validationRuleEntities = new[] {new ValidationRule()};

            var result = _target.Map(promocodeEntity, validationRuleEntities);

            Assert.AreEqual(DateTimeKind.Utc, result.ValidityStartDate.Kind);
            Assert.AreEqual(DateTimeKind.Utc, result.ValidityEndDate.Kind);
        }
    }
}
