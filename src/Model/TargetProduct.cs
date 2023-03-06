using System;

namespace Trainline.PromocodeService.Model
{
    public class TargetProduct
    {
        public Uri ProductUri { get; set; }

        public Money ProductAmount { get; set; }
    }
}
