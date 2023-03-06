namespace Trainline.PromocodeService.Host.Mappers
{
    public interface IRedemptionMapper
    {
        Contract.Redeemed Map(Model.Redeemed redeemed);

        Contract.Redemption Map(Model.Redemption redemption);
    }
}
