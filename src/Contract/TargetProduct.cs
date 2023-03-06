using System;
using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class TargetProduct
    {
        [Required]
        public Uri ProductUri { get; set; }

        [Required]
        public Money ProductAmount { get; set; }
    }
}
