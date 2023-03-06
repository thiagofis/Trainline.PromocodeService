using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service.Repository;
using Redeemed = Trainline.PromocodeService.Model.Redeemed;

namespace Trainline.PromocodeService.Service
{
    public class LedgerService : ILedgerService
    {
        private readonly ILedgerRepository _ledgerRepository;
        private readonly ILedgerMapper _ledgerMapper;

        private const decimal DefaultProportion = 0.75m;

        public LedgerService(ILedgerRepository ledgerRepository, ILedgerMapper ledgerMapper)
        {
            _ledgerRepository = ledgerRepository;
            _ledgerMapper = ledgerMapper;
        }

        public async Task<Ledger> Get(string promocodeId, string redemptionId)
        {
            var ledgerEntity = await _ledgerRepository.Get(promocodeId, redemptionId);
            return _ledgerMapper.Map(ledgerEntity);
        }

        public async Task<Ledger> Create(Redeemed redeemed)
        {
            var productItems = redeemed.InvoiceInfos.SelectMany(x => x.Items).ToList();

            var ledgerLines = productItems.Select(x => new LedgerLine
            {
                Amount = x.Amount,
                ProductUri = x.ProductUri
            });

            var products = productItems.Select(x => new ProductInfo
            {
                ProductPrice = x.ProductPrice,
                ProductUri = x.ProductUri,
                RootProductUri = x.ProductUri
            });

            var promoAmount = Math.Abs(productItems.Select(y => y.Amount).Sum());

            var ledger = new Ledger(
                redeemed.PromocodeId,
                redeemed.Id,
                redeemed.InvoiceInfos.First().CurrencyCode,
                promoAmount,
                ledgerLines,
                products);

            var ledgerEntity = _ledgerMapper.Map(ledger);

            await _ledgerRepository.Add(ledgerEntity);

            return ledger;
        }

        public async Task<ICollection<PromoQuote>> CreateQuotes(string promocodeId, string redemptionId, IEnumerable<QuoteRequest> quoteRequests)
        {
            var ledger = await Get(promocodeId, redemptionId);

            var productUri = quoteRequests.Select(x => x.ProductUri).FirstOrDefault();

            var existingProductUri = ledger.Products.Select(x => x.ProductUri)
                .Any(x => x.Equals(productUri));

            if (!existingProductUri)
            {
                throw new NotValidProductUriException($"{productUri} is not a valid ProductUri.");
            }

            var quotes = quoteRequests.Select(x => CreateQuote(ledger, x));

            var updatedQuotes = ledger.GetOrAdd(quotes).ToList();

            var ledgerEntity = _ledgerMapper.Map(ledger);

            await _ledgerRepository.Update(ledgerEntity);

            return updatedQuotes;
        }

        public async Task<PromoQuote> GetQuote(string promocodeId, string redemptionId, string quoteId)
        {
            var ledger = await Get(promocodeId, redemptionId);
            return ledger.Quotes.FirstOrDefault(x => x.PromoQuoteId == quoteId);
        }

        public async Task ForfeitQuote(string promocodeId, string redemptionId, string quoteId)
        {
            var ledger = await Get(promocodeId, redemptionId);
            ledger.ForfeitQuote(quoteId);

            var ledgerEntity = _ledgerMapper.Map(ledger);
            await _ledgerRepository.Update(ledgerEntity);
        }

        public async Task<PromoLink> Link(string promocodeId, string redemptionId, LinkProductRequest linkRequest)
        {
            var linkId = Guid.NewGuid().ToString();
            var ledger = await Get(promocodeId, redemptionId);

            if(linkRequest.OriginalProductAmount != null && ledger.CurrencyCode != linkRequest.OriginalProductAmount?.CurrencyCode)
            {
                throw new InvalidOperationException();
            }

            var existingProductUri =
                ledger.Products.Select(x => x.ProductUri).Any(x => x.Equals(linkRequest.OriginalProductUri));

            if (!existingProductUri)
            {
                throw new NotValidProductUriException($"{linkRequest.OriginalProductUri} is not a valid ProductUri.");
            }

            var existingProductLinks = ledger.Products.Where(x =>
                linkRequest.TargetProducts.Any(rp => rp.ProductUri  == x.ProductUri));
            if (existingProductLinks.Any())
            {
                var existingLinkId = existingProductLinks.Select(x => x.LinkId)
                    .Distinct()
                    .SingleOrDefault();

                if (existingLinkId == null)
                {
                    throw new InvalidOperationException();
                }

                return new PromoLink
                {
                    LinkId = existingLinkId,
                };
            }

            var originalProduct = ledger.Products.Single(x => x.ProductUri == linkRequest.OriginalProductUri);
            if (linkRequest.OriginalProductAmount == null)
            {
                var linkProducts = linkRequest.TargetProducts.Select(p =>  new ProductInfo()
                {
                    ProductPrice = p.ProductAmount.Amount,
                    ProductUri = p.ProductUri,
                    RootProductUri = originalProduct.RootProductUri,
                    LinkId = linkId,
                }).ToList();

                ledger.AddLinks(linkProducts, new List<LedgerLine>());
            }
            else
            {
                var targetProduct = linkRequest.TargetProducts.Single();
                var newProduct = new ProductInfo()
                {
                    ProductPrice = targetProduct.ProductAmount.Amount,
                    ProductUri = targetProduct.ProductUri,
                    RootProductUri = targetProduct.ProductUri,
                    LinkId = linkId,
                };

                var productPromoAmount = ledger.GetAmountUsedByProduct(linkRequest.OriginalProductUri);
                var productRemainingPromoAmount = ledger.GetRemainingAmountUsedByProduct(linkRequest.OriginalProductUri);
                var deductionAmount = CalculateProportionalDeduction(productRemainingPromoAmount, productPromoAmount, originalProduct.ProductPrice,
                    linkRequest.OriginalProductAmount.Amount);

                var linkLines = new List<LedgerLine>
                {
                    new LedgerLine
                    {
                        ProductUri = targetProduct.ProductUri,
                        Amount = -deductionAmount,
                        LinkId = linkId
                    },
                    new LedgerLine
                    {
                        ProductUri = linkRequest.OriginalProductUri,
                        Amount = deductionAmount,
                        LinkId = linkId
                    }
                };

                ledger.AddLink(newProduct, linkLines);
            }

            var ledgerEntity = _ledgerMapper.Map(ledger);
            await _ledgerRepository.Update(ledgerEntity);

            return new PromoLink
            {
                LinkId = linkId,
            };
        }
        
        public async Task RevertLink(string promocodeId, string redemptionId, string linkId) =>
            await _ledgerRepository.RemoveLink(promocodeId, redemptionId, linkId);
        

        private static PromoQuote CreateQuote(Ledger ledger, QuoteRequest quoteRequest)
        {
            var product = ledger.Products.Single(x => x.ProductUri == quoteRequest.ProductUri);
            var productPrice = product.ProductPrice;
            var productRemainingPromoAmount = ledger.GetRemainingAmountUsedByProduct(product.RootProductUri);
            var productPromoAmount = ledger.GetAmountUsedByProduct(product.RootProductUri);

            var deductionAmount = CalculateDeduction(productRemainingPromoAmount, productPromoAmount, productPrice, quoteRequest);

            return new PromoQuote
            {
                PromoQuoteId = Guid.NewGuid().ToString(),
                ReferenceId = quoteRequest.ReferenceId,
                ProductUri = product.RootProductUri,
                DeductionAmount = new Money(deductionAmount, ledger.CurrencyCode),
                Status = QuoteStatus.Pending
            };
        }

        private static decimal CalculateDeduction(decimal productRemainingPromoAmount, decimal productPromoAmount, decimal productPrice, QuoteRequest quoteRequest)
        {
            // rounding probably needs to be reworked here
            if (quoteRequest.ListedAmount == null)
            {
                var refundableAmount = quoteRequest.RefundableAmount;
                var maximumDeductible = Math.Round(refundableAmount.Amount * DefaultProportion, 2);

                return productRemainingPromoAmount > maximumDeductible
                    ? maximumDeductible
                    : productRemainingPromoAmount;
            }

            return CalculateProportionalDeduction(productRemainingPromoAmount, productPromoAmount, productPrice, quoteRequest.ListedAmount.Amount);
        }

        private static decimal CalculateProportionalDeduction(decimal productRemainingPromoAmount, decimal productPromoAmount, decimal productPrice, decimal productListedAmount)
        {
            var proportion = Math.Min(productListedAmount / productPrice, 1);

            return Math.Min(Math.Round(productPromoAmount * proportion, 2), productRemainingPromoAmount);
        }
    }
}
