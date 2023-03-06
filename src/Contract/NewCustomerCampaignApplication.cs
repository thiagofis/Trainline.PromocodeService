namespace Trainline.PromocodeService.Contract
{
    public class NewCustomerCampaignApplication
    {
        public NewCustomerCampaignApplication(string email, string firstName, string lastName, string externalCampaignId, string locale, string contextUri, string conversationId)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ExternalCampaignId = externalCampaignId;
            Locale = locale;
            ContextUri = contextUri;
            ConversationId = conversationId;
        }

        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string ExternalCampaignId { get; }
        public string ContextUri { get; }
        public string ConversationId { get; }
        public string Locale { get; }
    }
}
