using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trainline.PromocodeService.ExternalServices.Context.Contract
{
    public class ContextValues
    {
        [JsonProperty("Application:EntryPointId")]
        public string[] ApplicationEntryPointId { get; set; }

        [JsonProperty("Application:ManageGroupId")]
        public string[] ApplicationManageGroupId { get; set; }

        [JsonProperty("Application:ManagedGroupId")]
        public string[] ApplicationManagedGroupId { get; set; }

        [JsonProperty("Application:User:IpAddress")]
        public string[] ApplicationUserIpAddress { get; set; }

        [JsonProperty("Application:Language")]
        public string[] ApplicationLanguage { get; set; }

        [JsonProperty("Context:AliasId")]
        public string[] ContextAliasId { get; set; }
    }
}
