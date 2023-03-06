using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Trainline.NetStandard.StandardHeaders.Enums;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Host.UnitTests.Builders;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.DiscountStrategies;
using Trainline.PromocodeService.Service.Repository.Entities;
using ValidationRule = Trainline.PromocodeService.Service.Repository.Entities.ValidationRule;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.Product.SupportedProtocols;
using Promocode = Trainline.PromocodeService.Service.Repository.Entities.Promocode;

namespace Trainline.PromocodeService.Host.UnitTests
{
    [TestFixture]
    public class PromocodeServiceTests
    {
        private const string Code = "12345";
        private const string PromocodeId = "TestPromocodeId12345";
        private const string ValidationRuleId = "ValidationRule123124";
        private const string CampaignId = "TestCampaign";
        private readonly Uri ProductUri = new Uri("hppt://Product/Uri");
        private Service.PromocodeService _promocodeService;
        private Mock<IVoucherifyClient> _voucherifyClient;
        private Mock<IPromocodeRepository> _promocodeRepository;
        private Mock<IRedemptionRepository> _redemptionRepository;
        private Mock<ICampaignRepository> _campaignRepository;
        private IPromocodeMapper _promocodeMapper;
        private IVoucherifyMapper _voucherifyMapper;
        private RedemptionMapper _redemptionMapper;
        private Mock<IValidationRuleService> _validationRuleService;
        private ValidationRuleMapper _validationRuleMapper;
        private Mock<ILedgerService> _ledgerService;
        private Mock<ISupportedProtocolsService> _supportedProtocolsService;
        private Mock<IHeaderService> _headerService;
        private Promocode _promocodeEntity;


        [SetUp]
        public void Setup()
        {
            _voucherifyClient = new Mock<IVoucherifyClient>();
            _promocodeRepository = new Mock<IPromocodeRepository>();
            _promocodeRepository.Setup(x => x.Add(It.IsAny<Service.Repository.Entities.Promocode>()))
                .Returns<Service.Repository.Entities.Promocode>(x =>
                    Task.FromResult<Service.Repository.Entities.Promocode>(x));
            _redemptionRepository = new Mock<IRedemptionRepository>();
            _campaignRepository = new Mock<ICampaignRepository>();
            _validationRuleService = new Mock<IValidationRuleService>();
            _validationRuleService.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync(Enumerable.Empty<Service.Repository.Entities.ValidationRule>);
            _supportedProtocolsService = new Mock<ISupportedProtocolsService>();
            _supportedProtocolsService.Setup(x =>
                    x.SupportedProtocolsAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .ReturnsAsync(new string[] { "application/vnd.trainline.travelprotocol.v3+json" });
            _headerService = new Mock<IHeaderService>();
            _headerService.Setup(x => x.GetHeader(DefaultHeaders.ContextUri))
                .Returns(new string[] { "http://context/ui" });
            _headerService.Setup(x => x.GetHeader(DefaultHeaders.ConversationId)).Returns(new string[] { "conversationId" });

        _ledgerService = new Mock<ILedgerService>();

            _validationRuleMapper = new ValidationRuleMapper();
            _promocodeMapper = new PromocodeMapper(_validationRuleMapper);
            _redemptionMapper = new RedemptionMapper();

            var customerServiceClientMock = new Mock<ICustomerServiceClient>();
            var customerAttributeClientMock = new Mock<ICustomerAttributeClient>();
            var customerVoucherifyMapper = new CustomerVoucherifyMapper(customerServiceClientMock.Object, customerAttributeClientMock.Object);

            _voucherifyMapper = new VoucherifyMapper(customerVoucherifyMapper);

            var promocodeDiscountStrategyFactory = new PromocodeDiscountStrategyFactory(new List<IPromocodeDiscountStrategy>
            {
                new PromocodeAmountDiscountStrategy(),
                new PromocodePercentageDiscountStrategy()
            });
            var invoiceGenerator = new InvoiceGenerator(_promocodeMapper, promocodeDiscountStrategyFactory);
            _promocodeService = new Service.PromocodeService(_voucherifyClient.Object, _promocodeRepository.Object,
                _redemptionRepository.Object, _campaignRepository.Object, _promocodeMapper, _voucherifyMapper, _redemptionMapper, invoiceGenerator,
                _validationRuleService.Object, _ledgerService.Object, _headerService.Object);

            _promocodeEntity = new Promocode()
            {
                Code = Code,
                PromocodeId = PromocodeId,
                ValidationRuleId = ValidationRuleId,
                CampaignId = CampaignId,
                RetentionDate = new DateTime(2020, 01, 01)
            };
            _promocodeRepository.Setup(x => x.GetByPromocodeId(PromocodeId))
                .ReturnsAsync(_promocodeEntity);
        }

        [Test]
        public async Task Create_ExistingPromocode_ReturnsThatPromocode()
        {
            var promocodeEntity = CreatePromocodeEntity();
            _promocodeRepository.Setup(x => x.GetByCode(Code))
                .ReturnsAsync(() => promocodeEntity);

            var promocode = await _promocodeService.Create(Code);


            Assert.NotNull(promocode);
            Assert.AreEqual(promocodeEntity.Code, promocode.Code);
            Assert.AreEqual(promocodeEntity.Id, promocode.Id);
            Assert.AreEqual(promocodeEntity.CurrencyCode, promocode.CurrencyCode);
            Assert.AreEqual(promocodeEntity.ValidityStartDate, promocode.ValidityStartDate);
            Assert.AreEqual(promocodeEntity.ValidityEndDate, promocode.ValidityEndDate);
            Assert.AreEqual(promocodeEntity.DiscountAmount, promocode.Discount.Amount);
            Assert.AreEqual(promocodeEntity.DiscountType, promocode.Discount.Type);
            Assert.AreEqual(promocodeEntity.RedemptionQuantity, promocode.RedemptionQuantity);
            Assert.AreEqual(promocodeEntity.RedeemedQuantity, promocode.RedeemedQuantity);
            Assert.AreEqual(promocodeEntity.PromocodeId, promocode.PromocodeId);
        }


        [Test]
        public async Task Create_NonExistingPromocode_RetrievesFromVoucherify()
        {
            var voucher = new Voucher
            {
                Code = Code,
                StartDate = DateTime.UtcNow.AddDays(-5),
                ExpirationDate = DateTime.UtcNow.AddDays(5),
                Discount = new ExternalServices.Voucherify.Contract.Discount
                {
                    AmountOff = 100.00M.ToVoucherifyPrice(),
                    Type = DiscountTypeDefinitions.Amount
                },
                Redemption = new VoucherRedemptions
                {
                    RedeemedQuantity = 1,
                    Quantity = 10
                },
                Metadata = new Dictionary<string, string>
                {
                    { MetadataKeys.CurrencyCode, "GBP" },
                    {MetadataKeys.ProductType, "travel"}
                },
                ValidationRulesAssignments = new ValidationRulesAssignments
                {
                    Total = 0,
                    Data = new List<ValidationRulesAssignment>()
                },
                CampaignId = "TestCampaign",
                Active = true
            };

            _promocodeRepository.Setup(x => x.GetByCode(Code))
                .ReturnsAsync(() => null);

            _voucherifyClient.Setup(x => x.GetVoucher(Code))
                .ReturnsAsync(voucher);


            var promocode = await _promocodeService.Create(Code);


            Assert.NotNull(promocode);
            Assert.AreEqual(voucher.Code, promocode.Code);
            Assert.AreEqual(voucher.Metadata[MetadataKeys.CurrencyCode], promocode.CurrencyCode);
            Assert.AreEqual(voucher.StartDate, promocode.ValidityStartDate);
            Assert.AreEqual(voucher.ExpirationDate, promocode.ValidityEndDate);
            Assert.AreEqual(voucher.Discount.AmountOff.ToPromoPrice(), promocode.Discount.Amount);
            Assert.AreEqual(voucher.Discount.Type, promocode.Discount.Type);
            Assert.AreEqual(voucher.Redemption.Quantity, promocode.RedemptionQuantity);
            Assert.AreEqual(voucher.Redemption.RedeemedQuantity, promocode.RedeemedQuantity);
        }

        [Test]
        public async Task Create_CampaignNotExist_CreatesCampaign()
        {
            var campaignEntity = CreateCampaignEntity();

            var voucher = new Voucher
            {
                Code = Code,
                StartDate = DateTime.UtcNow.AddDays(-5),
                ExpirationDate = DateTime.UtcNow.AddDays(5),
                Discount = new ExternalServices.Voucherify.Contract.Discount
                {
                    AmountOff = 100.00M.ToVoucherifyPrice(),
                    Type = DiscountTypeDefinitions.Amount
                },
                Redemption = new VoucherRedemptions
                {
                    RedeemedQuantity = 1,
                    Quantity = 10
                },
                Metadata = new Dictionary<string, string>
                {
                    { MetadataKeys.CurrencyCode, "GBP" },
                    {MetadataKeys.ProductType, "travel" }
                },
                ValidationRulesAssignments = new ValidationRulesAssignments
                {
                    Total = 0,
                    Data = new List<ValidationRulesAssignment>()
                },
                CampaignId = "TestCampaign",
                Active = true
            };

            _promocodeRepository.Setup(x => x.GetByCode(Code))
                .ReturnsAsync(() => null);

            _campaignRepository.Setup(x => x.Get(voucher.CampaignId)).ReturnsAsync(() =>null);

            _campaignRepository.Setup(x => x.Add(new CampaignEntity {CampaignId = voucher.CampaignId})).ReturnsAsync(campaignEntity);

            _voucherifyClient.Setup(x => x.GetVoucher(Code))
                .ReturnsAsync(voucher);

            var promocode = await _promocodeService.Create(Code);

            _campaignRepository.Verify(x => x.Add(It.Is<CampaignEntity>(c => c.CampaignId.Equals(voucher.CampaignId))), Times.Once);

        }

        [Test]
        public async Task Create_ValidationRulesPersisted_ReturnsValidRulesFromDb()
        {
            var validationRuleId = "ruleid_1234";
            var promocodeEntity = CreatePromocodeEntity();
            promocodeEntity.ValidationRuleId = validationRuleId;

            _promocodeRepository.Setup(x => x.GetByCode(Code))
                .ReturnsAsync(() => promocodeEntity);

            _validationRuleService.Setup(x => x.Get(validationRuleId))
                .ReturnsAsync(new List<ValidationRule>
                {
                    new ValidationRule
                    {
                        RuleId = validationRuleId,
                        Name = ErrorMessagePrefix.OrderMinimumSpendPrefix,
                        Value = "2000"
                    },
                    new ValidationRule
                    {
                        RuleId = validationRuleId,
                        Name = ErrorMessagePrefix.VendorNotMatchingPrefix,
                        Value = "SNCF"
                    },
                    new ValidationRule
                    {
                        RuleId = validationRuleId,
                        Name = ErrorMessagePrefix.CurrencyCodeNotMatchingPrefix,
                        Value = "EUR"
                    }
                });

            var promocode = await _promocodeService.Create(Code);
            var validationRules = promocode.ValidationRules.ToList();

            Assert.NotNull(validationRules);
            Assert.AreEqual(5, validationRules.Count());

            var rule = validationRules.Single(x => x.Name == ErrorMessagePrefix.OrderMinimumSpendPrefix);
            Assert.AreEqual("2000", rule.Value);

            rule = validationRules.Single(x => x.Name == ErrorMessagePrefix.CurrencyCodeNotMatchingPrefix);
            Assert.AreEqual("EUR", rule.Value);

            rule = validationRules.Single(x => x.Name == ErrorMessagePrefix.VendorNotMatchingPrefix);
            Assert.AreEqual("SNCF", rule.Value);

            rule = validationRules.Single(x => x.Name == ValidationRuleMapper.PromocodeNotStarted);
            Assert.AreEqual(promocodeEntity.ValidityStartDate.ToString("o"), rule.Value);

            rule = validationRules.Single(x => x.Name == ValidationRuleMapper.PromocodeExpired);
            Assert.AreEqual(promocodeEntity.ValidityEndDate.ToString("o"), rule.Value);
        }

        [Test]
        public async Task Apply_ValidWithoutApplicableTo_ReturnsEmptyInvoice()
        {
            var invoice = InvoiceBuilder.Invoice()
                .WithLineItem("product123", 1000, ProductUri);


            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder.ForAmountDiscount(0));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single();
            Assert.AreEqual("InvoiceId_123", resultInvoice.Id);
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);
            Assert.IsEmpty(resultInvoice.Items);
        }

        [Test]
        public async Task Apply_ValidSingleApplicableTo_ReturnsDiscountInvoice()
        {
            const decimal discountValue = 10;
            const string productId = "productId_123";

            var invoice = InvoiceBuilder.Invoice()
                .WithLineItem(productId, 1000, ProductUri);

            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForProduct(productId));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single();
            Assert.AreEqual("InvoiceId_123", resultInvoice.Id);
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            var lineItem = resultInvoice.Items.Single();
            Assert.AreEqual(productId, lineItem.ProductId);
            Assert.AreEqual(-discountValue, lineItem.Amount);
        }

        [Test]
        public async Task Apply_TwoProductsSamePrice_ReturnsDiscountTwoDiscountsWithSameValue()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";

            var invoice = InvoiceBuilder.Invoice()
                .WithLineItem(productId1, 1000, ProductUri)
                .WithLineItem(productId2, 1000, ProductUri);

            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForProduct(productId1)
                    .ForProduct(productId2));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single();
            Assert.AreEqual("InvoiceId_123", resultInvoice.Id);
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            var lineItem = resultInvoice.Items.Single(x => x.ProductId == productId1);
            Assert.AreEqual(-discountValue / 2, lineItem.Amount);

            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId2);
            Assert.AreEqual(-discountValue / 2, lineItem.Amount);
        }

        [Test]
        public async Task Apply_TwoProducts_DiscountsProportionally()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";

            var invoice = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri)
                                                  .WithLineItem(productId2, 600, ProductUri);

            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForProduct(productId1)
                    .ForProduct(productId2));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single(); ;
            Assert.AreEqual("InvoiceId_123", resultInvoice.Id);
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            var lineItem = resultInvoice.Items.Single(x => x.ProductId == productId1);
            Assert.AreEqual(-4, lineItem.Amount);

            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId2);
            Assert.AreEqual(-6, lineItem.Amount);
        }

        [Test]
        public async Task Apply_TwoInvoiceProducts_DiscountsProportionally()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);
            var invoice2 = InvoiceBuilder.Invoice("InvoiceId_456").WithLineItem(productId2, 600, ProductUri);

            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForProduct(productId1)
                    .ForProduct(productId2));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice1, invoice2 });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single(x => x.Id == "InvoiceId_123");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            var lineItem = resultInvoice.Items.Single(x => x.ProductId == productId1);
            Assert.AreEqual(-4, lineItem.Amount);

            resultInvoice = invoices.DiscountInvoiceInfo.Single(x => x.Id == "InvoiceId_456");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId2);
            Assert.AreEqual(-6, lineItem.Amount);
        }

        [Test]
        public async Task ApplyPercentDiscount_TwoInvoiceProducts_DiscountsAllProducts()
        {
            const double percentDiscountValue = 10.0;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";
            const string productId3 = "productId_3";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);
            var invoice2 = InvoiceBuilder
                .Invoice("InvoiceId_456")
                .WithLineItem(productId2, 600, ProductUri)
                .WithLineItem(productId3, 800, ProductUri);

            _voucherifyClient.Setup(x => x.ValidateVoucher(Code, It.IsAny<Validation>()))
                .ReturnsAsync(VoucherValidatedBuilder
                    .ForPercentDiscount(percentDiscountValue)
                    .ForProduct(productId1)
                    .ForProduct(productId2)
                    .ForProduct(productId3));

            var invoices = await _promocodeService.Apply(PromocodeId, new InvoiceInfo[] { invoice1, invoice2 });

            var resultInvoice = invoices.DiscountInvoiceInfo.Single(x => x.Id == "InvoiceId_123");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);
            var lineItem = resultInvoice.Items.Single(x => x.ProductId == productId1);
            Assert.AreEqual(-40, lineItem.Amount);

            resultInvoice = invoices.DiscountInvoiceInfo.Single(x => x.Id == "InvoiceId_456");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);
            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId2);
            Assert.AreEqual(-60, lineItem.Amount);
            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId3);
            Assert.AreEqual(-80, lineItem.Amount);
        }

        [Test]
        public async Task Redeem_TwoInvoiceProducts_DiscountsAllProducts()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);
            var invoice2 = InvoiceBuilder.Invoice("InvoiceId_456").WithLineItem(productId2, 600, ProductUri);

            _voucherifyClient.Setup(x => x.RedeemVoucher(Code, It.IsAny<Redeem>()))
                .ReturnsAsync(VoucherRedeemedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 10, 10, "EUR", "travel")
                    .ForProduct(productId1)
                    .ForProduct(productId2));

            var redemption = await _promocodeService.Redeem(PromocodeId, new InvoiceInfo[] { invoice1, invoice2 }, DateTime.MaxValue);

            var resultInvoice = redemption.InvoiceInfos.Single(x => x.Id == "InvoiceId_123");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            var lineItem = resultInvoice.Items.Single(x => x.ProductId == productId1);
            Assert.AreEqual(-4, lineItem.Amount);

            resultInvoice = redemption.InvoiceInfos.Single(x => x.Id == "InvoiceId_456");
            Assert.AreEqual("GBP", resultInvoice.CurrencyCode);

            lineItem = resultInvoice.Items.Single(x => x.ProductId == productId2);
            Assert.AreEqual(-6, lineItem.Amount);
        }

        [Test]
        public async Task Redeem_ValidCall_UpdatesLocalVoucher()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";
            const string productId2 = "productId_2";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);
            var invoice2 = InvoiceBuilder.Invoice("InvoiceId_456").WithLineItem(productId2, 600, ProductUri);

            _voucherifyClient.Setup(x => x.RedeemVoucher(Code, It.IsAny<Redeem>()))
                .ReturnsAsync(VoucherRedeemedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 10, 10, "EUR", "travel")
                    .ForProduct(productId1)
                    .ForProduct(productId2));

            var invoices = await _promocodeService.Redeem(PromocodeId, new InvoiceInfo[] { invoice1, invoice2 }, DateTime.MaxValue);

            _promocodeRepository.Verify(x => x.Update(It.Is<Service.Repository.Entities.Promocode>(x =>
               x.Code == Code &&
               x.RedeemedQuantity == 10 &&
               x.RedemptionQuantity == 10 &&
               x.DiscountAmount == discountValue &&
               x.DiscountType == DiscountTypeDefinitions.Amount &&
               x.CurrencyCode == "EUR" &&
               x.ValidationRuleId == ValidationRuleId)), Times.Once);
        }

        [Test]
        public async Task Redeem_UpdatesWithValidRetentionDate()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);

            _voucherifyClient.Setup(x => x.RedeemVoucher(Code, It.IsAny<Redeem>()))
                .ReturnsAsync(VoucherRedeemedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 10, 10, "EUR", "travel")
                    .ForProduct(productId1));

            await _promocodeService.Redeem(PromocodeId, new InvoiceInfo[] { invoice1 }, DateTime.MaxValue);

            _promocodeRepository.Verify(x => x.Update(It.Is<Service.Repository.Entities.Promocode>(
                x => x.RetentionDate == DateTime.MaxValue)), Times.Once);
        }

        [Test]
        public async Task Redeem_AlwaysMaintainHigherRetentionDate()
        {
            const decimal discountValue = 10;
            const string productId1 = "productId_1";

            var invoice1 = InvoiceBuilder.Invoice().WithLineItem(productId1, 400, ProductUri);

            var lowerRetentionDate = _promocodeEntity.RetentionDate.AddMonths(-1);

            _voucherifyClient.Setup(x => x.RedeemVoucher(Code, It.IsAny<Redeem>()))
                .ReturnsAsync(VoucherRedeemedBuilder
                    .ForAmountDiscount(discountValue)
                    .ForPromocode(Code, 10, 10, "EUR", "travel")
                    .ForProduct(productId1));

            await _promocodeService.Redeem(PromocodeId, new InvoiceInfo[] { invoice1 }, lowerRetentionDate);

            _promocodeRepository.Verify(x => x.Update(It.Is<Service.Repository.Entities.Promocode>(
                p => p.RetentionDate == _promocodeEntity.RetentionDate)), Times.Once);
        }

        private Service.Repository.Entities.Promocode CreatePromocodeEntity(DateTime? startDate = null, DateTime? endDate = null)
        {
            var promocodeEntity = new Service.Repository.Entities.Promocode
            {
                Code = Code,
                Id = 123,
                CurrencyCode = "GBP",
                ValidityStartDate = startDate ?? DateTime.UtcNow.AddDays(-5),
                ValidityEndDate = endDate ?? DateTime.UtcNow.AddDays(5),
                DiscountAmount = 100,
                DiscountType = DiscountTypeDefinitions.Amount,
                RedeemedQuantity = 1,
                RedemptionQuantity = 10,
                ValidationRuleId = null,
                PromocodeId = PromocodeId,
                CampaignId = CampaignId
            };
            promocodeEntity.RetentionDate = promocodeEntity.ValidityEndDate.AddMonths(6);
            return promocodeEntity;
        }

        private Service.Repository.Entities.CampaignEntity CreateCampaignEntity(string campaignId = null, int? id = null)
        {
            var campaignEntity = new Service.Repository.Entities.CampaignEntity
            {
                Id = 1,
                CampaignId = CampaignId,
                Redeemable = true
            };
            return campaignEntity;
        }
    }
}
