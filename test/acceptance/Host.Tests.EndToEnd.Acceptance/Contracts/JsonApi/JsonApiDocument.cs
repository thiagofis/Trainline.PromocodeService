using System.Collections.Generic;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi
{
    public abstract class JsonApiDocument<TMappedType, TLinks, TRelationships> : PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi.JsonApiDocument
        where TMappedType : new()
        where TLinks : new()
        where TRelationships : new()
    {
        public DataElement<TMappedType, TLinks, TRelationships> Data { get; set; } =
            new DataElement<TMappedType, TLinks, TRelationships>();

        public List<IncludedData> Included { get; set; } = new List<IncludedData>();

        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();
    }

    public abstract partial class JsonApiDocument
    {
        public class DataElement<TAttributes, TLinks, TRelationships> : Ref
            where TLinks : new()
            where TAttributes : new()
            where TRelationships : new()
        {
            public TLinks Links { get; set; } = new TLinks();
            public TAttributes Attributes { get; set; } = new TAttributes();
            public TRelationships Relationships { get; set; } = new TRelationships();
        }

        public class RelationshipData
        {
            public Ref Data { get; set; }
        }

        public class Ref
        {
            public string Id { get; set; }
            public string Type { get; set; }
        }
    }
}
