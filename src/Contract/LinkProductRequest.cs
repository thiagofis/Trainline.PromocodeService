using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class LinkProductRequest
    {
        [Required]
        public Uri OriginalProductUri { get; set; }

        public Money OriginalProductAmount { get; set; }

        public List<TargetProduct> TargetProducts { get; set; }
    }
}
