namespace Trainline.PromocodeService.Host.Mappers
{
    public interface ILedgerMapper
    {
        Contract.Ledger Map(Model.Ledger ledger);

        Model.QuoteRequest Map(Contract.QuoteRequest quoteRequest);

        Contract.PromoQuote Map(string promocodeId, string redemptionId, Model.PromoQuote promoQuote);

        Model.LinkProductRequest Map(Contract.LinkProductRequest linkProductRequest);

        Contract.PromoLink Map(string promocodeId, string redemptionId, Model.PromoLink promoLink);
    }
}
