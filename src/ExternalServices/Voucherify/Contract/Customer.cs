namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Customer
    {
        public string source_id { get; set; }

        public CustomerMetadata metadata { get; set; } = new CustomerMetadata();

        public string name { get; set; }
    }

    public class CustomerMetadata
    {
        public string isnew_status { get; set; }
        public string railcard_status { get; set; }
        public string activity_status { get; set; }
    }
}
