namespace Trainline.PromocodeService.Contract
{
    public class NewCustomerCampaignEligibilityData
    {
        public NewCustomerCampaignEligibilityData(string customerId, string email, string firstName, string lastName, string externalCampaignId, string locale)
        {
            CustomerId = customerId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ExternalCampaignId = externalCampaignId;
            Locale = locale;
        }

        public string CustomerId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string ExternalCampaignId { get; }
        public string Locale { get; }
    }
}
