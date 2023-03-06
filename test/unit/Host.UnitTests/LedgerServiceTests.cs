using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Model;
using EntityLedger = Trainline.PromocodeService.Service.Repository.Entities.Ledger;
using EntityProductInfo = Trainline.PromocodeService.Service.Repository.Entities.ProductInfo;
using EntityLedgerLine = Trainline.PromocodeService.Service.Repository.Entities.LedgerLine;
using EntityPromoQuote = Trainline.PromocodeService.Service.Repository.Entities.PromoQuote;
using System.Collections.Generic;
using System;
using System.Linq;
using Trainline.PromocodeService.Service.Exceptions;

namespace Trainline.PromocodeService.Host.UnitTests
{
    [TestFixture]
    public class LedgerServiceTests
    {
        private LedgerService _sut;
        private Mock<ILedgerRepository> _ledgerRepository;
        private ILedgerMapper _ledgerMapper;
        private EntityLedger LedgerEntity => new EntityLedger
            {
                Id = 1,
                PromocodeId = "PromocodeId",
                RedemptionId = "RedemptionId",
                PromoAmount = 10m,
                CurrencyCode = "GBP",
                Products = new List<EntityProductInfo>
                {
                    new EntityProductInfo
                    {
                        Id = 1,
                        LedgerId = 1,
                        ProductPrice = 50m,
                        ProductUri = "http://www.products.com/one",
                        RootProductUri = "http://www.products.com/one"
                    },
                    new EntityProductInfo
                    {
                        Id = 2,
                        LedgerId = 1,
                        ProductPrice = 25m,
                        ProductUri = "http://www.products.com/two",
                        RootProductUri = "http://www.products.com/two"
                    },
                    new EntityProductInfo
                    {
                        Id = 3,
                        LedgerId = 1,
                        ProductPrice = 30m,
                        ProductUri = "http://www.products.com/linkToTwo",
                        RootProductUri = "http://www.products.com/two"
                    }
                },
                Lines = new List<EntityLedgerLine>
                {
                    new EntityLedgerLine
                    {
                        Id = 1,
                        LedgerId = 1,
                        ProductUri = "http://www.products.com/one",
                        Amount = -6.67m,
                    },
                    new EntityLedgerLine
                    {
                        Id = 2,
                        LedgerId = 1,
                        ProductUri = "http://www.products.com/two",
                        Amount = -3.33m,
                    },
                },
                Quotes = new List<EntityPromoQuote>()
            };

        [SetUp]
        public void Setup()
        {
            _ledgerRepository = new Mock<ILedgerRepository>();
            _ledgerMapper = new LedgerMapper();

            _sut = new LedgerService(_ledgerRepository.Object, _ledgerMapper);
        }
        
        [Test]
        public async Task Get_ExistingLedger_MapsFromEntity_ReturnsThatLedger()
        {
            var ledgerEntity = LedgerEntity;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var result = await _sut.Get("PromocodeId", "RedemptionId");

            Assert.AreEqual(ledgerEntity.Id, result.Id);
            Assert.AreEqual(ledgerEntity.PromocodeId, result.PromocodeId);
            Assert.AreEqual(ledgerEntity.RedemptionId, result.RedemptionId);
            Assert.AreEqual(ledgerEntity.CurrencyCode, result.CurrencyCode);
            Assert.AreEqual(ledgerEntity.PromoAmount, result.PromoAmount);
            AssertLines(ledgerEntity.Lines, result.Lines);
            AssertProducts(ledgerEntity.Products, result.Products);
        }

        [Test]
        public async Task Create_CreatesLedgerFromRedeemed()
        {
            var redeemed = new Redeemed
            {
                Id = "RedemptionId",
                Code = "PromoCode",
                PromocodeId = "PromocodeId",
                InvoiceInfos = new List<DiscountInvoiceInfo>
                {
                    new DiscountInvoiceInfo
                    {
                        Id = "Invoice",
                        CurrencyCode = "GBP",
                        Items = new List<DiscountItem>
                        {
                            new DiscountItem
                            {
                                ProductId = "one",
                                ProductUri = new Uri("http://www.products.com/one"),
                                Amount = -6.67m,
                                ProductPrice = 50m
                            },
                            new DiscountItem
                            {
                                ProductId = "two",
                                ProductUri = new Uri("http://www.products.com/two"),
                                Amount = -3.33m,
                                ProductPrice = 25m
                            },
                        }
                    }
                }
            };

            var result = await _sut.Create(redeemed);

            _ledgerRepository
                .Verify(x => x.Add(It.Is<EntityLedger>(l =>
                    l.Id == 0
                    && l.PromocodeId == result.PromocodeId
                    && l.RedemptionId == result.RedemptionId
                    && l.CurrencyCode == result.CurrencyCode
                    && AssertLines(l.Lines, result.Lines)
                    && AssertProducts(l.Products, result.Products))), Times.Once);

            Assert.AreEqual("PromocodeId", result.PromocodeId);
            Assert.AreEqual("RedemptionId", result.RedemptionId);
            Assert.AreEqual("GBP", result.CurrencyCode);
            Assert.AreEqual(10m, result.PromoAmount);
            Assert.AreEqual(0m, result.AvailableAmount);

            Assert.AreEqual(2, result.Products.Count);
            Assert.AreEqual(new Uri("http://www.products.com/one"), result.Products[0].ProductUri);
            Assert.AreEqual(50m, result.Products[0].ProductPrice);
            Assert.AreEqual(new Uri("http://www.products.com/two"), result.Products[1].ProductUri);
            Assert.AreEqual(25m, result.Products[1].ProductPrice);

            Assert.AreEqual(2, result.Lines.Count);
            Assert.AreEqual(new Uri("http://www.products.com/one"), result.Lines[0].ProductUri);
            Assert.AreEqual(-6.67m, result.Lines[0].Amount);
            Assert.AreEqual(new Uri("http://www.products.com/two"), result.Lines[1].ProductUri);
            Assert.AreEqual(-3.33m, result.Lines[1].Amount);

            Assert.AreEqual(0, result.Quotes.Count);
        }

        [Test]
        public async Task CreateQuotes_RequestsWithListedAmount_CreatesPromoQuotes()
        {
            var ledgerEntity = LedgerEntity;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var quoteRequests = new List<QuoteRequest>
            {
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/one"),
                    ReferenceId = "one-1",
                    ListedAmount = new Money(25m, "GBP"),
                    RefundableAmount = new Money(25m, "GBP"),
                },
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/two"),
                    ReferenceId = "two-1",
                    ListedAmount = new Money(25m, "GBP"),
                    RefundableAmount = new Money(25m, "GBP"),
                },
            };

            var quotes = await _sut.CreateQuotes("PromocodeId", "RedemptionId", quoteRequests);

            Assert.AreEqual(2, quotes.Count);

            var quote1 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/one"));
            Assert.AreEqual("one-1", quote1.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote1.Status);
            Assert.AreEqual(3.34m, quote1.DeductionAmount.Amount);

            var quote2 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/two"));
            Assert.AreEqual("two-1", quote2.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote2.Status);
            Assert.AreEqual(3.33m, quote2.DeductionAmount.Amount);
        }

        [Test]
        public async Task CreateQuotes_RequestWithoutListedAmount_CreatesPromoQuote()
        {
            var ledgerEntity = LedgerEntity;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var quoteRequests = new List<QuoteRequest>
            {
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/one"),
                    ReferenceId = "one-1",
                    RefundableAmount = new Money(1m, "GBP")
                }
            };

            var quotes = await _sut.CreateQuotes("PromocodeId", "RedemptionId", quoteRequests);

            Assert.AreEqual(1, quotes.Count);

            var quote1 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/one"));
            Assert.AreEqual("one-1", quote1.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote1.Status);
            Assert.AreEqual(0.75m, quote1.DeductionAmount.Amount);
        }

        [Test]
        public async Task CreateLink_RequestWithProductAmount_MoveCertainAmountToNewProduct()
        {
            var ledgerEntity = LedgerEntity;
            var targetProductUri = "http://www.products.com/target";
            var originalProductUri = ledgerEntity.Products[0].ProductUri;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var linkRequest = new LinkProductRequest
            {
                OriginalProductAmount = new Money(ledgerEntity.Products[0].ProductPrice / 2, ledgerEntity.CurrencyCode),
                OriginalProductUri = new Uri(originalProductUri),
                TargetProducts = new List<TargetProduct>
                {
                    new TargetProduct
                    {
                        ProductUri = new Uri(targetProductUri),
                        ProductAmount = new Money(50m, ledgerEntity.CurrencyCode)
                    }
                }
            };

            var promoLink = await _sut.Link("PromocodeId", "RedemptionId", linkRequest);

            _ledgerRepository
                .Verify(x => x.Update(It.Is<EntityLedger>(l =>
                    l.Products.SingleOrDefault(p =>
                        p.RootProductUri == targetProductUri
                        && p.ProductUri == targetProductUri
                        && p.LinkId == promoLink.LinkId
                        && p.ProductPrice == 50m
                        && p.Id == 0
                    ) != null
                    && l.Lines.Any(lo =>
                        lo.LinkId == promoLink.LinkId
                        && lo.Amount == 3.34m
                        && lo.ProductUri == originalProductUri
                        && lo.Id == 0
                    )
                    && l.Lines.Any(lr =>
                        lr.LinkId == promoLink.LinkId
                        && lr.Amount == -3.34m
                        && lr.ProductUri == targetProductUri
                        && lr.Id == 0
                    ))), Times.Once);
        }

        [Test]
        public void LedgerService_Throws_NotValidProductUriException_When_An_Unknown_ProducUri_Is_Passed()
        {
            var ledgerEntity = LedgerEntity;
            var targetProductUri = "http://www.products.com/target";
            var originalProductUri = "http://www.products.com/wronguri";
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var linkRequest = new LinkProductRequest
            {
                OriginalProductAmount = new Money(ledgerEntity.Products[0].ProductPrice / 2, ledgerEntity.CurrencyCode),
                OriginalProductUri = new Uri(originalProductUri),
                TargetProducts = new List<TargetProduct>
                {
                    new TargetProduct
                    {
                        ProductUri = new Uri(targetProductUri),
                        ProductAmount = new Money(50m, ledgerEntity.CurrencyCode)
                    }
                }
            };
            Assert.ThrowsAsync<NotValidProductUriException>(async () => await _sut.Link("PromocodeId", "RedemptionId", linkRequest)); 
        }

        [Test]
        public async Task CreateLink_RequestWithoutAmount_CreateLinkBetweenProducts()
        {
            var ledgerEntity = LedgerEntity;
            var targetProductUri = "http://www.products.com/target";
            var originalProductUri = ledgerEntity.Products[0].ProductUri;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var linkRequest = new LinkProductRequest
            {
                OriginalProductAmount = null,
                OriginalProductUri = new Uri(ledgerEntity.Products[0].ProductUri),
                TargetProducts = new List<TargetProduct>
                {
                    new TargetProduct
                    {
                        ProductUri = new Uri(targetProductUri),
                        ProductAmount = new Money(50m, ledgerEntity.CurrencyCode)
                    }
                }
            };

            var promoLink = await _sut.Link("PromocodeId", "RedemptionId", linkRequest);

            _ledgerRepository
                .Verify(x => x.Update(It.Is<EntityLedger>(l =>
                    l.Products.SingleOrDefault(p =>
                        p.RootProductUri == originalProductUri
                        && p.ProductUri == targetProductUri
                        && p.LinkId == promoLink.LinkId
                        && p.ProductPrice == 50m
                        && p.Id == 0
                    ) != null
                    && l.Lines.All(ledgerLine => ledgerLine.Id != 0))), Times.Once);
        }

        [Test]
        public async Task CreateQuotesForLinkedProduct_RequestWithoutListedAmount_CreatesPromoQuote()
        {
            var ledgerEntity = LedgerEntity;
            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var quoteRequests = new List<QuoteRequest>
            {
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/linkToTwo"),
                    ReferenceId = "link-2",
                    RefundableAmount = new Money(1m, "GBP")
                }
            };

            var quotes = await _sut.CreateQuotes("PromocodeId", "RedemptionId", quoteRequests);

            Assert.AreEqual(1, quotes.Count);

            var quote1 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/two"));
            Assert.AreEqual("link-2", quote1.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote1.Status);
            Assert.AreEqual(0.75m, quote1.DeductionAmount.Amount);
        }

        [Test]
        public async Task CreateQuotes_RequestsWithListedAmountHigherThenOriginalPrice_CreatesPromoQuotes()
        {
            var ledgerEntity = LedgerEntity;

            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var quoteRequests = new List<QuoteRequest>
            {
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/one"),
                    ReferenceId = "one-1",
                    ListedAmount = new Money(75m, "GBP"),
                    RefundableAmount = new Money(75m, "GBP"),
                }
            };

            var quotes = await _sut.CreateQuotes("PromocodeId", "RedemptionId", quoteRequests);

            Assert.AreEqual(1, quotes.Count);

            var quote1 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/one"));
            Assert.AreEqual("one-1", quote1.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote1.Status);
            Assert.AreEqual(6.67m, quote1.DeductionAmount.Amount);
        }

        [Test]
        public async Task CreateQuotes_RequestsWithListedAmountAndExistingLines_CreatesPromoQuotes()
        {
            var ledgerEntity = LedgerEntity;
            ledgerEntity.Lines.Add(
                new EntityLedgerLine
                {
                    Id = 1,
                    LedgerId = 1,
                    ProductUri = "http://www.products.com/one",
                    Amount = 3.67m,
                });

            _ledgerRepository
                .Setup(x => x.Get("PromocodeId", "RedemptionId"))
                .ReturnsAsync(ledgerEntity);

            var quoteRequests = new List<QuoteRequest>
            {
                new QuoteRequest
                {
                    ProductUri = new Uri("http://www.products.com/one"),
                    ReferenceId = "one-1",
                    ListedAmount = new Money(25m, "GBP"),
                    RefundableAmount = new Money(25m, "GBP"),
                }
            };

            var quotes = await _sut.CreateQuotes("PromocodeId", "RedemptionId", quoteRequests);

            Assert.AreEqual(1, quotes.Count);

            var quote1 = quotes.Single(x => x.ProductUri == new Uri("http://www.products.com/one"));
            Assert.AreEqual("one-1", quote1.ReferenceId);
            Assert.AreEqual(QuoteStatus.Pending, quote1.Status);
            Assert.AreEqual(3m, quote1.DeductionAmount.Amount);
        }

        private static bool AssertLines(IReadOnlyList<EntityLedgerLine> entityLines, IReadOnlyList<LedgerLine> modelLines)
        {
            Assert.AreEqual(entityLines.Count, modelLines.Count);
            for (int i = 0; i < entityLines.Count; i++)
            {
                Assert.AreEqual(entityLines[i].Id, modelLines[i].Id);
                Assert.AreEqual(entityLines[i].Amount, modelLines[i].Amount);
                Assert.AreEqual(entityLines[i].ProductUri, modelLines[i].ProductUri.ToString());
            }

            return true;
        }

        private static bool AssertProducts(IReadOnlyList<EntityProductInfo> entityProducts, IReadOnlyList<ProductInfo> modelProducts)
        {
            Assert.AreEqual(entityProducts.Count, modelProducts.Count);
            for (int i = 0; i < entityProducts.Count; i++)
            {
                Assert.AreEqual(entityProducts[i].Id, modelProducts[i].Id);
                Assert.AreEqual(entityProducts[i].ProductUri, modelProducts[i].ProductUri.ToString());
                Assert.AreEqual(entityProducts[i].ProductPrice, modelProducts[i].ProductPrice);
            }

            return true;
        }
    }
}
