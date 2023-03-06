namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class FailedResponse
    {
        public string Key { get; set; }

        public RedeemedError Error { get; set; }
    }

    public class RedeemedError
    {
        public string Message { get; set; }

    }
}
