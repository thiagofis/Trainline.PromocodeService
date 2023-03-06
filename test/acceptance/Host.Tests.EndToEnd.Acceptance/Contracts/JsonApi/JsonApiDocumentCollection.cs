using System.Collections.Generic;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi
{
    public abstract class JsonApiDocumentCollection<TMappedType, TLinks, TRelationships> : PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi.JsonApiDocument
        where TMappedType : new()
        where TLinks : new()
        where TRelationships : new()
    {
        public List<DataElement<TMappedType, TLinks, TRelationships>> Data { get; set; } =
            new List<DataElement<TMappedType, TLinks, TRelationships>>();

        public List<IncludedData> Included { get; set; } = new List<IncludedData>();

        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();
    }
}
