using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Host.UnitTests.Builders;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.PromocodeService.Service.Repository.Entities;
using Redemption = Trainline.PromocodeService.Service.Repository.Entities.Redemption;

namespace Trainline.PromocodeService.Host.UnitTests
{
    [TestFixture]
    public class RedemptionServiceTests
    {
        private const string Code = "12345";
        private const string PromocodeId = "PromocodeId12345";
        private const string RedemptionId = "Redemption_12345";
        private const string ValidationRuleId = "ValidationRuleId234234";
        private RedemptionService _service;
        private Mock<IVoucherifyClient> _voucherifyClient;
        private Mock<IPromocodeRepository> _promocodeRepository;
        private Mock<IRedemptionRepository> _redemptionRepository;
        private IPromocodeMapper _promocodeMapper;
        private RedemptionMapper _redemptionMapper;


        [SetUp]
        public void Setup()
        {
            _voucherifyClient = new Mock<IVoucherifyClient>();
            _promocodeRepository = new Mock<IPromocodeRepository>();
            _redemptionRepository = new Mock<IRedemptionRepository>();
            var validationRuleMapper = new ValidationRuleMapper();
            _promocodeMapper = new PromocodeMapper(validationRuleMapper);
            _redemptionMapper = new RedemptionMapper();

            _service = new RedemptionService(_voucherifyClient.Object, _promocodeRepository.Object,
                _redemptionRepository.Object, _promocodeMapper, _redemptionMapper);

            _promocodeRepository.Setup(x => x.GetByPromocodeId(PromocodeId))
                .ReturnsAsync(new Promocode() { Code = Code, PromocodeId = PromocodeId, ValidationRuleId = ValidationRuleId});
        }

        [Test]
        public async Task Get_ExistingRedemption_ReturnsThatRedemption()
        {
            _redemptionRepository.Setup(x => x.Get(PromocodeId, RedemptionId))
                .ReturnsAsync(() => new Redemption
                {
                    RedemptionId = RedemptionId,
                    PromocodeId = PromocodeId
                });

            var redemption = await _service.Get(PromocodeId, RedemptionId);

            Assert.AreEqual(PromocodeId, redemption.PromocodeId);
            Assert.AreEqual(RedemptionId, redemption.Id);
        }

        [Test]
        public async Task Get_NotExistingRedemption_ReturnsNull()
        {
            _redemptionRepository.Setup(x => x.Get(PromocodeId, RedemptionId))
                .ReturnsAsync(() => null);

            var redemption = await _service.Get(PromocodeId, RedemptionId);

            Assert.IsNull(redemption);
        }

        [Test]
        public async Task GetAll_ExistingRedemption_ReturnsThatRedemption()
        {
            _redemptionRepository.Setup(x => x.GetByPromocodeId(PromocodeId))
                .ReturnsAsync(() =>new [] {new Redemption
                {
                    PromocodeId = PromocodeId,
                    RedemptionId = RedemptionId
                }});

            var redemptions = await _service.GetByPromocodeId(PromocodeId);

            var redemption = redemptions.Single();
            Assert.AreEqual(PromocodeId, PromocodeId);
            Assert.AreEqual(RedemptionId, redemption.Id);
        }

        [Test]
        public async Task GetAll_NotExistingRedemption_ReturnsNull()
        {
            _redemptionRepository.Setup(x => x.GetByPromocodeId(PromocodeId))
                .ReturnsAsync(() => null);

            var redemptions = await _service.GetByPromocodeId(PromocodeId);

            Assert.IsNull(redemptions);
        }

        [Test]
        public async Task Reinstate_ValidCall_UpdatesLocalVoucher()
        {
            const decimal discountValue = 10;

            _voucherifyClient.Setup(x => x.RollbackVoucher(Code, RedemptionId))
                .ReturnsAsync(VoucherRollbackBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 1, 10, "EUR", "travel"));

            await _service.Reinstate(PromocodeId, RedemptionId);

            _promocodeRepository.Verify(x => x.Update(It.Is<Service.Repository.Entities.Promocode>(x =>
                x.Code == Code &&
                x.RedeemedQuantity == 1 &&
                x.RedemptionQuantity == 10 &&
                x.DiscountAmount == discountValue &&
                x.PromocodeId == PromocodeId &&
                x.DiscountType == DiscountTypeDefinitions.Amount &&
                x.CurrencyCode == "EUR" &&
                x.ValidationRuleId == ValidationRuleId &&
                x.ProductType == "travel" )), Times.Once);
        }

        [Test]
        public void Reinstate_InvalidCall_ThrowsNotApplicableException()
        {
            const decimal discountValue = 10;

            _voucherifyClient.Setup(x => x.RollbackVoucher(Code, RedemptionId))
                .ReturnsAsync(VoucherRollbackBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 1, 10, "EUR", "travel")
                    .ForResult("FAILED"));

            Assert.ThrowsAsync<NotApplicableException>(async () => await _service.Reinstate(PromocodeId, RedemptionId));

            _promocodeRepository.Verify(x => x.Update(It.IsAny<Service.Repository.Entities.Promocode>()), Times.Never);
        }

        [Test]
        public void GivenAnNonExistingPromocode_WhenReinstateIt_ThenAPromocodeNotFoundExceptionIsThrown()
        {
            var nonExistingPromocodeId = "nonExistingPromocodeId";

            AsyncTestDelegate action = async () => await _service.Reinstate(nonExistingPromocodeId, RedemptionId);

            Assert.ThrowsAsync<PromocodeNotFoundException>(action);
        }
    }
}
