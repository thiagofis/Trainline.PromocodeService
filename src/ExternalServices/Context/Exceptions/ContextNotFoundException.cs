using System;

namespace Trainline.PromocodeService.ExternalServices.Context.Exceptions
{
    public class ContextNotFoundException : Exception
    {
        public ContextNotFoundException(string contextUri) : base($"It is not possible to find the context with Uri '{contextUri}'.")
        {
        }
    }
}
