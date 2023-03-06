using System;
using System.Runtime.Serialization;
using Trainline.NetStandard.Exceptions.Model;

namespace Trainline.PromocodeService.Host.Exceptions
{
    [Serializable]
    public class ValidationServiceException : ServiceException
    {
        public ValidationServiceException(string message) : base("InvalidRequest", message, Severity.Correctable) { }
        protected ValidationServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
