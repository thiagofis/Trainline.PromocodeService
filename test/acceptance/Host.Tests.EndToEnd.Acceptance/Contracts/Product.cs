using System;
using System.Collections.Generic;
using Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts
{
    public class Product : JsonApiDocument<Product.Attributes, Product.Links, Product.Relationships>
    {
        public class Attributes
        {
            public DateTime? ExpiresAt { get; set; }
            public ListPrice ListPrice { get; set; }
            public string State { get; set; }
            public string VendorName { get; set; }
        }

        public class Links : Dictionary<string, Link>
        {
            public Link Self => this[nameof(Self).ToLower()];
            public Link DeliveryMethods => this["deliveryMethods"];
            public Link Lock => this[nameof(Lock).ToLower()];
            public Link Issue => this[nameof(Issue).ToLower()];
        }

        public class Relationships
        {
            public RelationshipData SelectedDeliveryOption { get; set; }
        }
    }
}
