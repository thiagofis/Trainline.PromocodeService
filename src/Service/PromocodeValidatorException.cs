using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Service
{
    public class PromocodeValidatorException : Exception
    {
        public IEnumerable<string> ErrorCodes { get; }

        public PromocodeValidatorException(IEnumerable<string> errorCodes)
        {
            this.ErrorCodes = errorCodes;
        }
    }
}
