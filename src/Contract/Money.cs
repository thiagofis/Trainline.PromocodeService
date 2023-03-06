using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class Money
    {
        [Required]
        public decimal? Amount { get; set; }

        [Required]
        public string CurrencyCode { get; set; }
    }
}
