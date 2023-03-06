using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Service.Exceptions
{
    public class MultipleCurrencyNotSupportedException : Exception
    {
        private List<string> currencyCodes;

        public MultipleCurrencyNotSupportedException(List<string> currencyCodes)
            :base("Multiple currencies not supported.")
        {
            this.currencyCodes = currencyCodes;
        }
    }
}
