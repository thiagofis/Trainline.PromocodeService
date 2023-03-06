using Newtonsoft.Json;

namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class EntryParameter
    {
        public EntryParameter(string entryPointId, string managedGroupId)
        {
            EntryPointId = entryPointId;
            ManagedGroupId = managedGroupId;
        }

        [JsonProperty("entryPointId")]
        public string EntryPointId { get; }

        [JsonProperty("managedGroupId")]
        public string ManagedGroupId { get; }

        public override string ToString()
        {
            return $"{nameof(EntryPointId)}: {EntryPointId}, {nameof(ManagedGroupId)}: {ManagedGroupId}";
        }
    }
}
