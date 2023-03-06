namespace Trainline.PromocodeService.ExternalServices.CardType.Contract
{
    public class Error
    {
        public int HttpStatusCode { get; set; }

        public string Key { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }
    }
}
