using System;
using System.Collections.Generic;
using System.Linq;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class LedgerMapper : ILedgerMapper
    {
        public Ledger Map(Model.Ledger ledger)
            => new Ledger
            {
                Id = ledger.Id,
                PromocodeId = ledger.PromocodeId,
                RedemptionId = ledger.RedemptionId,
                CurrencyCode = ledger.CurrencyCode,
                PromoAmount = ledger.PromoAmount,
                Lines = ledger.Lines.Select(Map).ToList(),
                Products = ledger.Products.Select(Map).ToList(),
                Quotes = ledger.Quotes.Select(Map).ToList(),
            };

        public Model.Ledger Map(Ledger ledger)
            => new Model.Ledger(
                ledger.Id,
                ledger.PromocodeId,
                ledger.RedemptionId,
                ledger.CurrencyCode,
                ledger.PromoAmount,
                ledger.Lines.Select(Map),
                ledger.Products.Select(Map),
                ledger.Quotes.Select(Map));

        private static Model.LedgerLine Map(LedgerLine ledgerLine)
            => new Model.LedgerLine
            {
                Id = ledgerLine.Id,
                Amount = ledgerLine.Amount,
                ProductUri = new Uri(ledgerLine.ProductUri),
                LinkId = ledgerLine.LinkId
            };

        private static LedgerLine Map(Model.LedgerLine ledgerLine)
            => new LedgerLine
            {
                Id = ledgerLine.Id,
                Amount = ledgerLine.Amount,
                ProductUri = ledgerLine.ProductUri.ToString(),
                LinkId = ledgerLine.LinkId,
            };

        private static Model.ProductInfo Map(ProductInfo productInfo)
            => new Model.ProductInfo
            {
                Id = productInfo.Id,
                ProductPrice = productInfo.ProductPrice,
                ProductUri = new Uri(productInfo.ProductUri),
                RootProductUri = new Uri(productInfo.RootProductUri),
                LinkId = productInfo.LinkId
            };

        private static ProductInfo Map(Model.ProductInfo productInfo)
            => new ProductInfo
            {
                Id = productInfo.Id,
                ProductPrice = productInfo.ProductPrice,
                ProductUri = productInfo.ProductUri.ToString(),
                RootProductUri = productInfo.RootProductUri.ToString(),
                LinkId = productInfo.LinkId
            };

        private static Model.PromoQuote Map(PromoQuote promoQuote)
            => new Model.PromoQuote
            {
                Id = promoQuote.Id,
                PromoQuoteId = promoQuote.PromoQuoteId,
                ReferenceId = promoQuote.ReferenceId,
                ProductUri = new Uri(promoQuote.ProductUri),
                Hash = promoQuote.Hash,
                DeductionAmount = new Model.Money(promoQuote.DeductionAmount, promoQuote.DeductionCurrencyCode),
                Status = (Model.QuoteStatus)promoQuote.Status
            };

        private static PromoQuote Map(Model.PromoQuote promoQuote)
            => new PromoQuote
            {
                Id = promoQuote.Id,
                PromoQuoteId = promoQuote.PromoQuoteId,
                ReferenceId = promoQuote.ReferenceId,
                ProductUri = promoQuote.ProductUri.ToString(),
                Hash = promoQuote.Hash,
                DeductionAmount = promoQuote.DeductionAmount.Amount,
                DeductionCurrencyCode = promoQuote.DeductionAmount.CurrencyCode,
                Status = (short)promoQuote.Status
            };
    }
}
