using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Model
{
    public class LinkProductRequest
    {
        public Uri OriginalProductUri { get; set; }

        public Money OriginalProductAmount { get; set; }

        public List<TargetProduct> TargetProducts { get; set; }
    }
}
