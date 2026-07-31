using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using LibraryMS.Application.Contracts.Common;

namespace LibraryMS.HttpApi.Host.Swagger;

/// <summary>
/// A global Swagger operation filter that automatically documents common error response types (400, 401, 403, 404, 500)
/// along with their standardized response schema structure (ApiResponse of object) to keep contracts consistent.
/// </summary>
public sealed class ProducesCommonResponseTypesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Generate the OpenAPI Schema metadata for our standard failure response structure: ApiResponse<object>
        var errorSchema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse<object>), context.SchemaRepository);
        
        var jsonMediaType = new OpenApiMediaType
        {
            Schema = errorSchema
        };

        // 1. Add 400 Bad Request globally
        if (!operation.Responses.ContainsKey("400"))
        {
            var response = new OpenApiResponse 
            { 
                Description = "Bad Request - Invalid parameters or validation errors." 
            };
            response.Content.Add("application/json", jsonMediaType);
            operation.Responses.Add("400", response);
        }

        // 2. Add 401 and 403 response documentation for authenticated endpoints
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasAllowAnonymous = endpointMetadata.Any(m => m is AllowAnonymousAttribute);
        var hasAuthorize = endpointMetadata.Any(m => m is AuthorizeAttribute);

        if (!hasAllowAnonymous && hasAuthorize)
        {
            if (!operation.Responses.ContainsKey("401"))
            {
                var response = new OpenApiResponse 
                { 
                    Description = "Unauthorized - JWT access token is missing or expired." 
                };
                response.Content.Add("application/json", jsonMediaType);
                operation.Responses.Add("401", response);
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                var response = new OpenApiResponse 
                { 
                    Description = "Forbidden - You do not have the required permissions or role." 
                };
                response.Content.Add("application/json", jsonMediaType);
                operation.Responses.Add("403", response);
            }
        }

        // 3. Add 404 Not Found for resource queries or manipulations
        var httpMethod = context.ApiDescription.HttpMethod;
        var targetsResource = httpMethod == "GET" || httpMethod == "PUT" || httpMethod == "DELETE" || httpMethod == "PATCH";
        if (targetsResource && !operation.Responses.ContainsKey("404"))
        {
            var response = new OpenApiResponse 
            { 
                Description = "Not Found - The requested resource was not found." 
            };
            response.Content.Add("application/json", jsonMediaType);
            operation.Responses.Add("404", response);
        }

        // 4. Add 500 Internal Server Error globally
        if (!operation.Responses.ContainsKey("500"))
        {
            var response = new OpenApiResponse 
            { 
                Description = "Internal Server Error - An unexpected backend error occurred." 
            };
            response.Content.Add("application/json", jsonMediaType);
            operation.Responses.Add("500", response);
        }
    }
}
