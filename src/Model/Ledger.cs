using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Trainline.PromocodeService.Model
{
    public class Ledger
    {
        public long Id { get; set; }

        public string PromocodeId { get; set; }

        public string RedemptionId { get; set; }

        public string CurrencyCode { get; set; }

        public decimal PromoAmount { get; set; }

        public decimal AvailableAmount => PromoAmount + Lines.Select(x => x.Amount).Sum();

        public IReadOnlyList<LedgerLine> Lines => _lines.AsReadOnly();
        private readonly List<LedgerLine> _lines;

        public IReadOnlyList<ProductInfo> Products => _products.AsReadOnly();
        private readonly List<ProductInfo> _products;

        public IReadOnlyList<PromoQuote> Quotes => _quotes.AsReadOnly();
        private readonly List<PromoQuote> _quotes;

        public Ledger(
            string promocodeId,
            string redemptionId,
            string currencyCode,
            decimal promoAmount,
            IEnumerable<LedgerLine> ledgerLines,
            IEnumerable<ProductInfo> products)
        {
            PromocodeId = promocodeId;
            RedemptionId = redemptionId;
            CurrencyCode = currencyCode;
            PromoAmount = promoAmount;
            _lines = ledgerLines.ToList();
            _products = products.ToList();
            _quotes = new List<PromoQuote>();
        }

        public Ledger(
            long id,
            string promocodeId,
            string redemptionId,
            string currencyCode,
            decimal promoAmount,
            IEnumerable<LedgerLine> ledgerLines,
            IEnumerable<ProductInfo> products,
            IEnumerable<PromoQuote> quotes)
            : this(promocodeId, redemptionId, currencyCode, promoAmount, ledgerLines, products)
        {
            Id = id;
            _quotes = quotes.ToList();
        }
        public decimal GetRemainingAmountUsedByProduct(Uri productUri)
            => Math.Abs(Lines.Where(x => x.ProductUri == productUri).Select(x => x.Amount).Sum());


        public decimal GetAmountUsedByProduct(Uri productUri)
            => Math.Abs(Lines.Where(x => x.ProductUri == productUri && x.Amount < 0).Select(x => x.Amount).Sum());

        public void ForfeitQuote(string quoteId)
        {
            var quote = Quotes.Single(x => x.PromoQuoteId == quoteId);
            if (quote.Status == QuoteStatus.PromoValueForfeited)
            {
                return;
            }

            if (quote.Status == QuoteStatus.Invalid)
            {
                throw new ForfeitQuoteException(PromocodeId, RedemptionId, quote.PromoQuoteId, quote.ReferenceId);
            }

            var newLedgerLine = new LedgerLine
            {
                Amount = quote.DeductionAmount.Amount,
                ProductUri = quote.ProductUri
            };

            _lines.Add(newLedgerLine);

            foreach (var quoteToInvalidate in Quotes.Where(x => x.ProductUri == quote.ProductUri && x.ReferenceId == quote.ReferenceId))
            {
                quoteToInvalidate.Status = QuoteStatus.Invalid;
            }
            quote.Status = QuoteStatus.PromoValueForfeited;
        }

        public IEnumerable<PromoQuote> GetOrAdd(IEnumerable<PromoQuote> promoQuotes)
        {
            foreach (var promoQuote in promoQuotes)
            {
                yield return GetOrAdd(promoQuote);
            }
        }

        public PromoQuote GetOrAdd(PromoQuote promoQuote)
        {
            promoQuote.Hash = HashPromoQuote(promoQuote);
            var storedQuote = _quotes.FirstOrDefault(x => x.Hash == promoQuote.Hash);
            if (storedQuote != null)
            {
                return storedQuote;
            }

            _quotes.Add(promoQuote);
            return promoQuote;
        }

        public void AddLink(ProductInfo product, IList<LedgerLine> lines)
        {
            _products.Add(product);
            _lines.AddRange(lines);
        }

        public void AddLinks(IList<ProductInfo> products, IList<LedgerLine> lines)
        {
            _products.AddRange(products);
            _lines.AddRange(lines);
        }

        public void RevertLink(string linkId)
        {
            _products.RemoveAll(x => x.LinkId != linkId);
            _lines.RemoveAll(x => x.LinkId != linkId);
        }

        private static string HashPromoQuote(PromoQuote promoQuote)
        {
            var promoQuoteString = $"{promoQuote.ReferenceId}--{promoQuote.ProductUri}--{promoQuote.DeductionAmount}";

            var hashableBytes = Encoding.UTF8.GetBytes(promoQuoteString);

            using var md5 = MD5.Create();
            var result = md5.ComputeHash(hashableBytes);

            var sb = new StringBuilder();
            foreach (var t in result)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
