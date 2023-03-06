using System.Linq;
using System.Text.Json;

namespace Trainline.PromocodeService.ExternalServices.Context.Contract
{
    public class ContextParameters
    {
        public string Id { get; set; }
        public ContextValues Values { get; set; }
        public string EntryPointId => Values.ApplicationEntryPointId.FirstOrDefault();

        public string ManagedGroupId => Values.ApplicationManagedGroupId.FirstOrDefault() ??
                                        Values.ApplicationManageGroupId.FirstOrDefault();

    }
}
