using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.App.SwaggerConfig;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters.Where(p => p.In == ParameterLocation.Query))
        {
            var fileParameter = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.ModelMetadata?.ModelType == typeof(IFormFile));

            if (fileParameter != null)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    [fileParameter.Name] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                Required = new HashSet<string> { fileParameter.Name }
                            }
                        }
                    }
                };
            }
        }
    }
}