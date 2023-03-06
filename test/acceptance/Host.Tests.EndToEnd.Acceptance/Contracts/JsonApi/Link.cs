namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi
{
    public abstract partial class JsonApiDocument
    {
        public class Link
        {
            public string Href { get; set; }
            public LinkMeta Meta { get; set; } = new LinkMeta();

            public class LinkMeta
            {
                public string Method { get; set; }
            }
        }
    }
}
