namespace Trainline.PromocodeService.Service.Mappers
{
    public interface ILedgerMapper
    {
        Repository.Entities.Ledger Map(Model.Ledger ledger);

        Model.Ledger Map(Repository.Entities.Ledger ledger);
    }
}
