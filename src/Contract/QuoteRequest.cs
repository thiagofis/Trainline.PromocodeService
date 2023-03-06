using System;
using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class QuoteRequest
    {
        [Required]
        public Uri ProductUri { get; set; }

        [Required]
        public string ReferenceId { get; set; }

        [Required]
        public Money RefundableAmount { get; set; }

        public Money ListedAmount { get; set; }
    }
}
