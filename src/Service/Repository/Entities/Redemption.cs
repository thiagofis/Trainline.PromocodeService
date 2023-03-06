using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class Redemption
    {
        public int Id { get; set; }
        public string RedemptionId { get; set; }
        public string Code { get; set; }
        public string PromocodeId { get; set; }
    }
}
