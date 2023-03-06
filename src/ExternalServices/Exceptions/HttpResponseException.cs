using System;
using System.Net;

namespace Trainline.PromocodeService.ExternalServices.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(Uri uri, HttpStatusCode statusCode, string content) : base(content)
        {
            Uri = uri;
            StatusCode = statusCode;
        }

        public Uri Uri { get; }
        public HttpStatusCode StatusCode { get; }


        public override string Message => $"{nameof(base.Message)}: {base.Message}, {nameof(Uri)}: {Uri}, {nameof(StatusCode)}: {StatusCode}";
    }
}
