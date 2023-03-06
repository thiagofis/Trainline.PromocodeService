namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class Error
    {
        public string AdditionalInfo { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return $"{nameof(Code)}: {Code}, {nameof(Message)}: {Message}";
        }
    }
}
