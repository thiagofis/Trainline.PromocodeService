using System;

namespace Trainline.PromocodeService.Contract
{
    public class RedeemPromocode
    {
        public Invoice[] Invoices { get; set; }

        public DateTime? RetentionDate { get; set; }
    }
}
