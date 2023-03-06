using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.DiscountStrategies;
using Trainline.PromocodeService.Service.Mappers;
using Discount = Trainline.PromocodeService.ExternalServices.Voucherify.Contract.Discount;

namespace Service.UnitTests
{
    public class InvoiceGeneratorTests
    {
        private InvoiceGenerator _invoiceGenerator;
        private Mock<IPromocodeMapper> _promocodeMapper;
        private Mock<IPromocodeDiscountStrategy> _discountStrategy;

        public InvoiceGeneratorTests()
        {
            _promocodeMapper = new Mock<IPromocodeMapper>();
            _promocodeMapper.Setup(x => x.Map(It.IsAny<Discount>()))
                .Returns<Discount>(d => new Trainline.PromocodeService.Model.Discount
                {
                    Amount = d.AmountOff.ToPromoPrice(),
                    Type = d.Type
                });
            _discountStrategy = new Mock<IPromocodeDiscountStrategy>();
            _discountStrategy.Setup(x => x.GenerateDiscountItems(It.IsAny<Trainline.PromocodeService.Model.Discount>(),
                    It.IsAny<ICollection<ProductItem>>()))
                .Returns<Trainline.PromocodeService.Model.Discount, ICollection<ProductItem>>((d, ps) =>
                    ps.Select(p => new DiscountItem
                    {
                        Amount = -d.Amount / ps.Count,
                        ProductId = p.ProductId,
                        ProductPrice = p.Amount
                    }));
            var discountStrategyFactory = new Mock<IPromocodeDiscountStrategyFactory>();
            discountStrategyFactory.Setup(x => x.Get(
                    It.IsAny<Trainline.PromocodeService.Model.Discount>()))
                .Returns(() => _discountStrategy.Object);
            _invoiceGenerator = new InvoiceGenerator(_promocodeMapper.Object, discountStrategyFactory.Object);
        }

        [Test]
        public void Generate_ReturnInvoiceItemForEachApplicableProduct()
        {
            var response = TestVoucherifyResponse.CreateAmountResponse(10, "1", "2");
            var invoiceInfos = new List<InvoiceInfo>()
            {
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "1", 10m), new KeyValuePair<string, decimal>( "2", 20m))
            };

            var invoices = _invoiceGenerator.Generate(response, invoiceInfos);

            Assert.AreEqual(1, invoices.Count());
            var invoice = invoices.Single();

            Assert.AreEqual(2, invoice.Items.Count);
            var productIds = invoice.Items.Select(x => x.ProductId).ToList();

            Assert.Contains("1", productIds);
            Assert.Contains("2", productIds);
            Assert.AreEqual(10m, -invoice.Items.Sum(p => p.Amount));
        }

        [Test]
        public void Generate_WithSecondTierResponse_ReturnInvoiceItemForEachApplicableProduct()
        {
            var response = TestVoucherifyResponse.CreateAmountResponse(10, "1", "2");
            response.Metadata = new Dictionary<string, string>
            {
                {InvoiceGenerator.TiersEnableMetadataKey, "true" },
                {InvoiceGenerator.TierTwoDiscountMetadataKey, "20" },
                {InvoiceGenerator.TierTwoThresholdMetadataKey, "40" }
            };

            var invoiceInfos = new List<InvoiceInfo>()
            {
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "1", 20m)),
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "2", 20m))
            };

            var invoices = _invoiceGenerator.Generate(response, invoiceInfos);

            Assert.AreEqual(2, invoices.Count());
            var productItems = invoices.SelectMany(x => x.Items).ToList();
            Assert.NotNull(productItems.SingleOrDefault(x => x.ProductId == "1"));
            Assert.NotNull(productItems.SingleOrDefault(x => x.ProductId == "2"));
            Assert.AreEqual(20m, -productItems.Sum(p => p.Amount));
        }

        [Test]
        public void Generate_WithSecondTierResponseSwitchedOff_ReturnInvoiceItemForEachApplicableProduct()
        {
            var response = TestVoucherifyResponse.CreateAmountResponse(10, "1", "2");
            response.Metadata = new Dictionary<string, string>
            {
                {InvoiceGenerator.TiersEnableMetadataKey, "false" },
                {InvoiceGenerator.TierTwoDiscountMetadataKey, "20" },
                {InvoiceGenerator.TierTwoThresholdMetadataKey, "40" }
            };

            var invoiceInfos = new List<InvoiceInfo>()
            {
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "1", 20m)),
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "2", 20m))
            };

            var invoices = _invoiceGenerator.Generate(response, invoiceInfos);

            Assert.AreEqual(2, invoices.Count());
            var productItems = invoices.SelectMany(x => x.Items).ToList();
            Assert.NotNull(productItems.SingleOrDefault(x => x.ProductId == "1"));
            Assert.NotNull(productItems.SingleOrDefault(x => x.ProductId == "2"));
            Assert.AreEqual(10m, -productItems.Sum(p => p.Amount));
        }

        [Test]
        public void Generate_MissingConfiguration_ThrowsException()
        {
            var response = TestVoucherifyResponse.CreateAmountResponse(10, "1", "2");
            response.Metadata = new Dictionary<string, string>
            {
                {InvoiceGenerator.TiersEnableMetadataKey, "true" },
                {InvoiceGenerator.TierTwoDiscountMetadataKey, "twenty" },
                {InvoiceGenerator.TierTwoThresholdMetadataKey, "forty" }
            };

            var invoiceInfos = new List<InvoiceInfo>()
            {
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "1", 20m)),
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "2", 20m))
            };

            Assert.Throws<InvalidOperationException>(() => _invoiceGenerator.Generate(response, invoiceInfos));
        }

        [Test]
        public void Generate_UnprocessableConfiguration_ThrowsException()
        {
            var response = TestVoucherifyResponse.CreateAmountResponse(10, "1", "2");
            response.Metadata = new Dictionary<string, string>
            {
                {InvoiceGenerator.TiersEnableMetadataKey, "true" },
            };

            var invoiceInfos = new List<InvoiceInfo>()
            {
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "1", 20m)),
                GetProductsInInvoice(new KeyValuePair<string, decimal>( "2", 20m))
            };

            Assert.Throws<InvalidOperationException>(() => _invoiceGenerator.Generate(response, invoiceInfos));
        }

        private InvoiceInfo GetProductsInInvoice(params KeyValuePair<string, decimal>[] productIdsWithPrices)
        {
            return new InvoiceInfo
            {
                Id = Guid.NewGuid().ToString(),
                CurrencyCode = "GBP",
                ProductItems = productIdsWithPrices.Select(x => 
                    new ProductItem
                    {
                        Amount = x.Value,
                        ProductId = x.Key
                    }
                ).ToList()
            };
        }


        public class TestVoucherifyResponse : IVoucherifyResponse
        {
            public Discount Discount { get; set; }
            public ApplicableTo ApplicableTo { get; set; }
            public Dictionary<string, string> Metadata { get; set; }

            public static TestVoucherifyResponse CreateAmountResponse(decimal amount, params string[] productIds)
            {
                return new TestVoucherifyResponse
                {
                    Discount = new Discount
                    {
                        AmountOff = amount.ToVoucherifyPrice(),
                        Type = DiscountTypeDefinitions.Amount
                    },
                    ApplicableTo = new ApplicableTo
                    {
                        Object = Guid.NewGuid().ToString(),
                        Data = productIds.Select(x => new ApplicableInfo
                        {
                            SourceId = x
                        }).ToList(),
                        Total = productIds.Length
                    } 
                };
            }
        }
    }
}
