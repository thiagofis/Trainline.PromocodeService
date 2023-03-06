using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Constants;

namespace Trainline.PromocodeService.Host.Mappers
{
    public class LedgerMapper : ILedgerMapper
    {
        private readonly IUrlHelper _urlHelper;

        public LedgerMapper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public Ledger Map(Model.Ledger ledger)
            => new Ledger
            {
                PromocodeId = ledger.PromocodeId,
                RedemptionId = ledger.RedemptionId,
                CurrencyCode = ledger.CurrencyCode,
                AvailableAmount = ledger.AvailableAmount,
                PromoAmount = ledger.PromoAmount,
                Lines = ledger.Lines.Select(Map).ToList(),
                Quotes = ledger.Quotes.Select(x => Map(ledger.PromocodeId, ledger.RedemptionId, x)).ToList(),
                Products = ledger.Products.Select(Map).ToList(),
                Links = CreateLinks(ledger)
            };

        public PromoQuote Map(string promocodeId, string redemptionId, Model.PromoQuote promoQuote)
            => new PromoQuote
            {
                Id = promoQuote.PromoQuoteId,
                ProductUri = promoQuote.ProductUri,
                ReferenceId = promoQuote.ReferenceId,
                DeductionAmount = Map(promoQuote.DeductionAmount),
                Status = Map(promoQuote.Status),
                Links = CreateLinks(promocodeId, redemptionId, promoQuote)
            };

        public Model.QuoteRequest Map(QuoteRequest quoteRequest)
            => new Model.QuoteRequest
            {
                ReferenceId = quoteRequest.ReferenceId,
                ProductUri = quoteRequest.ProductUri,
                ListedAmount = Map(quoteRequest.ListedAmount),
                RefundableAmount = Map(quoteRequest.RefundableAmount),
            };

        public Model.LinkProductRequest Map(LinkProductRequest linkProductRequest)
            => new Model.LinkProductRequest
            {
                OriginalProductUri = linkProductRequest.OriginalProductUri,
                OriginalProductAmount = Map(linkProductRequest.OriginalProductAmount),
                TargetProducts = linkProductRequest.TargetProducts.Select(Map).ToList()
            };

        public Contract.PromoLink Map(string promocodeId, string redemptionId, Model.PromoLink promoLink)
            => new Contract.PromoLink
            {
                Id = promoLink.LinkId,
                RedemptionId = redemptionId,
                Links = CreateLinks(promocodeId, redemptionId, promoLink)
            };

        private static Model.TargetProduct Map(TargetProduct targetProduct)
            => new Model.TargetProduct
            {
                ProductUri = targetProduct.ProductUri,
                ProductAmount = Map(targetProduct.ProductAmount)
            };

        private Dictionary<string, Link> CreateLinks(Model.Ledger ledger)
        {
            var routeValues = new { promocodeId = ledger.PromocodeId, redemptionId = ledger.RedemptionId };

            return new Dictionary<string, Link>
            {
                ["self"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetLedger, routeValues)),
                    Method = HttpMethod.Get.Method
                },
                ["quotes"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.CreateQuote, routeValues)),
                    Method = HttpMethod.Post.Method
                }
            };
        }

        private Dictionary<string, Link> CreateLinks(string promocodeId, string redemptionId, Model.PromoQuote promoQuote)
        {
            var routeValues = new { promocodeId = promocodeId, redemptionId = redemptionId, quoteId = promoQuote.PromoQuoteId};

            var links = new Dictionary<string, Link>
            {
                ["self"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetQuote, routeValues)),
                    Method = HttpMethod.Get.Method
                }
            };

            if (promoQuote.Status == Model.QuoteStatus.Pending)
            {
                links["forfeit"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.ForfeitQuote, routeValues)),
                    Method = HttpMethod.Post.Method
                };
            }

            return links;
        }

        private Dictionary<string, Link> CreateLinks(string promocodeId, string redemptionId, Model.PromoLink promoLink)
        {
            var routeValuesForRevert = new { promocodeId = promocodeId, redemptionId = redemptionId, linkId = promoLink.LinkId };
            var routeValues = new { promocodeId = promocodeId, redemptionId = redemptionId };

            var links = new Dictionary<string, Link>
            {
                ["link"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.CreateLink, routeValues)),
                    Method = HttpMethod.Post.Method
                },
                ["revert"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.RevertLink, routeValuesForRevert)),
                    Method = HttpMethod.Post.Method
                },
                ["ledger"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetLedger, routeValues)),
                    Method = HttpMethod.Get.Method
                },
                ["quotes"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.CreateQuote, routeValues)),
                    Method = HttpMethod.Post.Method
                }
            };

            return links;
        }

        private static Money Map(Model.Money money)
            => money == null ? null : new Money
            {
                Amount = money.Amount,
                CurrencyCode = money.CurrencyCode
            };

        private static Model.Money Map(Money money)
            => money == null
                ? null
                : new Model.Money(money.Amount.Value, money.CurrencyCode);

        private static LedgerLine Map(Model.LedgerLine ledgerLine)
            => new LedgerLine
            {
                Amount = ledgerLine.Amount,
                ProductUri = ledgerLine.ProductUri,
                LinkId = ledgerLine.LinkId
            };

        private static ProductInfo Map(Model.ProductInfo productInfo)
            => new ProductInfo
            {
                ProductUri = productInfo.ProductUri,
                ProductPrice = productInfo.ProductPrice,
                RootProductUri = productInfo.RootProductUri,
                LinkId = productInfo.LinkId
            };

        private static QuoteStatus Map(Model.QuoteStatus quoteStatus)
            => quoteStatus switch
            {
                Model.QuoteStatus.PromoValueForfeited => QuoteStatus.PromoValueForfeited,
                Model.QuoteStatus.Invalid => QuoteStatus.Invalid,
                Model.QuoteStatus.Pending => QuoteStatus.Pending,
            };
    }
}
