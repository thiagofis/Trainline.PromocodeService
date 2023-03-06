namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute
{
    public class CustomerAttributeSettings
    {
        public string BaseUri { get; set; }

        public bool IsServiceEnabled => !string.IsNullOrWhiteSpace(BaseUri);
    }
}
