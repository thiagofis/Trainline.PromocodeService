using Newtonsoft.Json;
using Trainline.PromocodeService.Common.Formatting;

namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class SearchCriteria
    {
        public SearchCriteria(string emailAddress, string managedGroupId)
        {
            EmailAddress = emailAddress;
            ManagedGroupId = managedGroupId;
        }

        public string EmailAddress { get; }

        [JsonProperty("ManagedGroupID")]
        public string ManagedGroupId { get; }

        public override string ToString()
        {
            return $"{nameof(EmailAddress)}: {EmailAddress.MaskEmail()}, {nameof(ManagedGroupId)}: {ManagedGroupId}";
        }
    }
}
