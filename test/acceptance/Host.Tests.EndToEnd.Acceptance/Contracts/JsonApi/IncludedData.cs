using System.Collections.Generic;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi
{
    public abstract partial class JsonApiDocument
    {
        public class IncludedData : Ref
        {
            public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        }
    }
}
