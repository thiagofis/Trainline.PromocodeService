using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace Trainline.PromocodeService.Host.Startup
{
    public class TrainlineMediaTypeApiVersionReader : MediaTypeApiVersionReader
    {
        private const string DefaultServiceName = "promocode";
        private readonly Regex _regex;

        public TrainlineMediaTypeApiVersionReader(string serviceName = DefaultServiceName) 
        {
            string pattern = $@"application\/vnd\.trainline\.{serviceName}\.v(\d)\+json";
            _regex = new Regex(pattern);
        }

        protected override string? ReadAcceptHeader(ICollection<MediaTypeHeaderValue> accept)
        {
            var version = 0;
            foreach (var mediaTypeHeaderValue in accept)
            {
                var match = _regex.Match(mediaTypeHeaderValue.MediaType.Value);

                if (match.Success && match.Groups.Count > 1)
                {
                    version = Math.Max(version, int.Parse(match.Groups[1].Value));
                }
            }

            return version > 0 ? $"{version}.0" : base.ReadAcceptHeader(accept);
        }
        
        public override string? Read(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var headers = request.GetTypedHeaders();
            var accept = headers.Accept;
            return accept == null ? default : ReadAcceptHeader(accept);
        }
    }
}
