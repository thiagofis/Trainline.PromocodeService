using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{

    public class VoucherifySettings
    {
        public string BaseUri { get; set; }

        public string AppId { get; set; }

        public string AppToken { get; set; }
    }
}
