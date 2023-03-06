using System;

namespace Trainline.PromocodeService.ExternalServices.Http.Requests
{
    public class HttpResult<T>
    {
        public T Result { get; set; }

        public Uri Location { get; set; }

        public string MediaType { get; set; }
    }
}
