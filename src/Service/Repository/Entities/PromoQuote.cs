namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class PromoQuote
    {
        public long Id { get; set; }

        public long LedgerId { get; set; }

        public string PromoQuoteId { get; set; }

        public string ReferenceId { get; set; }

        public string ProductUri { get; set; }

        public decimal DeductionAmount { get; set; }

        public string DeductionCurrencyCode { get; set; }

        public short Status { get; set; }

        public string Hash { get; set; }
    }
}
