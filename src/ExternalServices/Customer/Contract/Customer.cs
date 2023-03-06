namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class Customer
    {
        public string Id { get; set; }
        public string EmailId { get; set; }
        public int HomeManagedGroupId { get; set; }
        public string ForeName { get; set; }
        public string SurName { get; set; }
    }
}
