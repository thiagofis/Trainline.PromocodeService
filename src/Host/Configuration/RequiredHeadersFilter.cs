using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Trainline.PromocodeService.Host.Configuration
{
    public class RequiredHeadersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "ConversationId",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("SwaggerConversationId")
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "ContextUri",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("https://some-context-uri")
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "User-Agent",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("Swagger/1.2.3")
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "CustomerUri",
                In = ParameterLocation.Header,
                Required = false,
                Example = new OpenApiString("https://some-customer-uri")
            });
        }
    }
}
